using Logistics.Domain.Common.ValueObjects;
using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Entities.Employees;
using Logistics.Persistence;
using Logistics.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Tests;

[TestClass]
public sealed class PersistenceTests
{
    private static DataContext CreateContext(string databaseName)
    {
        DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        DataContext context = new(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static async Task<(DataContext context, Employee employee, Credential credential, AccessPoint accessPoint)> CreateSeedDataAsync(string databaseName)
    {
        DataContext context = CreateContext(databaseName);

        var employee = new Employee("1020304050", "Daniel", "Zapata", Email.Create("daniel.zapata@logistics.com"), "Operario de Bodega", OrganizationalArea.Warehouse);
        var inactiveEmployee = new Employee("1122334455", "Pedro", "Sánchez", Email.Create("pedro.sanchez@logistics.com"), "Operario de Bodega", OrganizationalArea.Warehouse);
        inactiveEmployee.Deactivate();

        var credential = new Credential("CRD-0001", employee.Id);
        var accessPoint = new AccessPoint("Recepción Principal", AccessPointType.Reception);

        await context.Employees.AddRangeAsync(employee, inactiveEmployee);
        await context.Credentials.AddAsync(credential);
        await context.AccessPoints.AddAsync(accessPoint);
        await context.AccessRecords.AddRangeAsync(
            AccessRecord.CreateAuthorized(credential.Id, accessPoint.Id, MovementType.Entry),
            AccessRecord.CreateDenied(credential.Id, accessPoint.Id, MovementType.Exit, DenialReason.CredentialBlocked));
        await context.SaveChangesAsync();

        return (context, employee, credential, accessPoint);
    }

    [TestMethod]
    public async Task DataContext_WithCompleteGraph_ShouldPersistAllEntities()
    {
        var (context, employee, credential, accessPoint) = await CreateSeedDataAsync("LogisticsTests_Graph");

        Assert.AreEqual(2, await context.Employees.CountAsync());
        Assert.AreEqual(1, await context.Credentials.CountAsync());
        Assert.AreEqual(1, await context.AccessPoints.CountAsync());
        Assert.AreEqual(2, await context.AccessRecords.CountAsync());
        Assert.AreEqual(credential.EmployeeId, employee.Id);
        Assert.IsTrue(context.Employees.Any(e => e.Status == EmploymentStatus.Inactive));
    }

    [TestMethod]
    public async Task EmployeesRepository_GetPagedListAsync_ShouldFilterByStatus()
    {
        var (context, _, _, _) = await CreateSeedDataAsync("LogisticsTests_EmployeesFilter");
        var repository = new EmployeesRepository(context);

        var activeEmployees = await repository.GetPagedListAsync(new(1, 15), status: EmploymentStatus.Active);

        Assert.AreEqual(1, activeEmployees.TotalCount);
        Assert.AreEqual("Daniel", activeEmployees.Items[0].Names);
    }

    [TestMethod]
    public async Task EmployeesRepository_GetByIdWithCredentialsAsync_ShouldIncludeCredentials()
    {
        var (context, employee, credential, _) = await CreateSeedDataAsync("LogisticsTests_EmployeeCredentials");
        var repository = new EmployeesRepository(context);

        var result = await repository.GetByIdWithCredentialsAsync(employee.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Credentials.Count);
        Assert.AreEqual(credential.Code, result.Credentials.First().Code);
    }

    [TestMethod]
    public async Task CredentialsRepository_GetByCodeWithEmployeeAsync_ShouldReturnCredentialAndEmployee()
    {
        var (context, employee, _, _) = await CreateSeedDataAsync("LogisticsTests_CredentialByCode");
        var repository = new CredentialsRepository(context);

        var credential = await repository.GetByCodeWithEmployeeAsync("crd-0001");

        Assert.IsNotNull(credential);
        Assert.AreEqual("CRD-0001", credential.Code);
        Assert.IsNotNull(credential.Employee);
        Assert.AreEqual(employee.Id, credential.Employee.Id);
    }

    [TestMethod]
    public async Task AccessRecordsRepository_GetPagedListAsync_ShouldFilterByResult()
    {
        var (context, _, credential, accessPoint) = await CreateSeedDataAsync("LogisticsTests_RecordsFilter");
        var repository = new AccessRecordsRepository(context);

        var deniedRecords = await repository.GetPagedListAsync(new(1, 15), credentialId: credential.Id, result: AccessResult.Denied);

        Assert.AreEqual(1, deniedRecords.TotalCount);
        Assert.AreEqual(DenialReason.CredentialBlocked, deniedRecords.Items[0].DeniedReason);
    }

    [TestMethod]
    public async Task AccessPointsRepository_GetPagedListAsync_ShouldFilterByEnabled()
    {
        var (context, _, _, accessPoint) = await CreateSeedDataAsync("LogisticsTests_PointsFilter");
        accessPoint.Disable();
        context.AccessPoints.Update(accessPoint);
        await context.SaveChangesAsync();
        var repository = new AccessPointsRepository(context);

        var disabledPoints = await repository.GetPagedListAsync(new(1, 15), isEnabled: false);

        Assert.AreEqual(1, disabledPoints.TotalCount);
        Assert.AreEqual("Recepción Principal", disabledPoints.Items[0].Name);
    }
}

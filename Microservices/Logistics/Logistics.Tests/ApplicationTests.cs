using Logistics.Application.Contracts.Persistence;
using Logistics.Application.Contracts.Repositories;
using Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess;
using Logistics.Application.UseCases.Employees.Commands.CreateEmployee;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeeById;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeesList;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Common.ValueObjects;
using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Entities.Employees;
using Logistics.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Logistics.Tests;

[TestClass]
public sealed class ApplicationTests
{
    private class FakeUnitOfWork : IUnitOfWork
    {
        public int CommitCount { get; private set; }

        public Task CommitAsync()
        {
            CommitCount++;
            return Task.CompletedTask;
        }

        public Task RollbackAsync() => Task.CompletedTask;
    }

    private class FakeEmployeesRepository : IEmployeesRepository
    {
        private readonly List<Employee> _employees;

        public FakeEmployeesRepository(List<Employee> employees)
        {
            _employees = employees;
        }

        public Task<Employee> CreateAsync(Employee entity, CancellationToken cancellationToken = default)
        {
            _employees.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<Employee> UpdateAsync(Employee entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);
        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<Employee>> GetListAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Employee>>(_employees);
        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));
        public Task<Employee?> GetByIdWithCredentialsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));

        public Task<bool> ExistsByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_employees.Any(e => e.IdentificationNumber == identificationNumber.Trim()));
        }

        public Task<PaginationResponse<Employee>> GetPagedListAsync(PaginationRequest request,
                                                                    EmploymentStatus? status = null,
                                                                    OrganizationalArea? area = null,
                                                                    string? searchTerm = null,
                                                                    CancellationToken cancellationToken = default)
        {
            var query = _employees.AsQueryable();
            if (status.HasValue) query = query.Where(e => e.Status == status.Value);
            if (area.HasValue) query = query.Where(e => e.Area == area.Value);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e => e.Names.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                      || e.LastNames.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                      || e.IdentificationNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            var items = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            return Task.FromResult(PaginationResponse<Employee>.Create(items, query.Count(), request));
        }
    }

    private class FakeCredentialsRepository : ICredentialsRepository
    {
        private readonly List<Credential> _credentials;

        public FakeCredentialsRepository(List<Credential> credentials)
        {
            _credentials = credentials;
        }

        public Task<Credential> CreateAsync(Credential entity, CancellationToken cancellationToken = default)
        {
            _credentials.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<Credential> UpdateAsync(Credential entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);
        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<Credential>> GetListAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Credential>>(_credentials);
        public Task<Credential?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_credentials.FirstOrDefault(c => c.Id == id));

        public Task<Credential?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            string cleanCode = code.Trim().ToUpperInvariant();
            return Task.FromResult(_credentials.FirstOrDefault(c => c.Code == cleanCode));
        }

        public Task<Credential?> GetByCodeWithEmployeeAsync(string code, CancellationToken cancellationToken = default) => GetByCodeAsync(code);

        public Task<PaginationResponse<Credential>> GetPagedListByEmployeeAsync(Guid employeeId,
                                                                                PaginationRequest request,
                                                                                CancellationToken cancellationToken = default)
        {
            var query = _credentials.Where(c => c.EmployeeId == employeeId);
            var items = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            return Task.FromResult(PaginationResponse<Credential>.Create(items, query.Count(), request));
        }
    }

    private class FakeAccessPointsRepository : IAccessPointsRepository
    {
        private readonly List<AccessPoint> _accessPoints;

        public FakeAccessPointsRepository(List<AccessPoint> accessPoints)
        {
            _accessPoints = accessPoints;
        }

        public Task<AccessPoint> CreateAsync(AccessPoint entity, CancellationToken cancellationToken = default)
        {
            _accessPoints.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<AccessPoint> UpdateAsync(AccessPoint entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);
        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<AccessPoint>> GetListAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<AccessPoint>>(_accessPoints);
        public Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_accessPoints.FirstOrDefault(p => p.Id == id));

        public Task<PaginationResponse<AccessPoint>> GetPagedListAsync(PaginationRequest request,
                                                                       AccessPointType? type = null,
                                                                       bool? isEnabled = null,
                                                                       CancellationToken cancellationToken = default)
        {
            var query = _accessPoints.AsQueryable();
            if (type.HasValue) query = query.Where(p => p.Type == type.Value);
            if (isEnabled.HasValue) query = query.Where(p => p.IsEnabled == isEnabled.Value);

            var items = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            return Task.FromResult(PaginationResponse<AccessPoint>.Create(items, query.Count(), request));
        }
    }

    private class FakeAccessRecordsRepository : IAccessRecordsRepository
    {
        private readonly List<AccessRecord> _records;

        public FakeAccessRecordsRepository()
        {
            _records = [];
        }

        public List<AccessRecord> CreatedRecords => _records;

        public Task<AccessRecord> CreateAsync(AccessRecord entity, CancellationToken cancellationToken = default)
        {
            _records.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<AccessRecord> UpdateAsync(AccessRecord entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);
        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IEnumerable<AccessRecord>> GetListAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<AccessRecord>>(_records);
        public Task<AccessRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_records.FirstOrDefault(r => r.Id == id));

        public Task<PaginationResponse<AccessRecord>> GetPagedListAsync(PaginationRequest request,
                                                                        Guid? employeeId = null,
                                                                        Guid? credentialId = null,
                                                                        Guid? accessPointId = null,
                                                                        AccessResult? result = null,
                                                                        DateTime? dateFrom = null,
                                                                        DateTime? dateTo = null,
                                                                        CancellationToken cancellationToken = default)
        {
            var query = _records.AsQueryable();
            if (credentialId.HasValue) query = query.Where(r => r.CredentialId == credentialId.Value);
            if (accessPointId.HasValue) query = query.Where(r => r.AccessPointId == accessPointId.Value);
            if (result.HasValue) query = query.Where(r => r.Result == result.Value);
            if (dateFrom.HasValue) query = query.Where(r => r.OccurredAt >= dateFrom.Value);
            if (dateTo.HasValue) query = query.Where(r => r.OccurredAt <= dateTo.Value);

            var items = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            return Task.FromResult(PaginationResponse<AccessRecord>.Create(items, query.Count(), request));
        }
    }

    private (FakeEmployeesRepository employees,
             FakeCredentialsRepository credentials,
             FakeAccessPointsRepository accessPoints,
             FakeAccessRecordsRepository records,
             FakeUnitOfWork unitOfWork,
             Employee employee,
             Credential credential,
             AccessPoint accessPoint) CreateAccessTestData()
    {
        var employee = new Employee("1020304050", "Daniel", "Zapata", Email.Create("daniel.zapata@logistics.com"), "Operario de bodega", OrganizationalArea.Warehouse);
        var credential = new Credential("CRD-0001", employee.Id);
        var accessPoint = new AccessPoint("Recepción principal", AccessPointType.Reception);

        // Set navigation property using reflection for testing
        typeof(Credential).GetProperty("Employee")?.SetValue(credential, employee);

        return (new FakeEmployeesRepository([employee]),
                new FakeCredentialsRepository([credential]),
                new FakeAccessPointsRepository([accessPoint]),
                new FakeAccessRecordsRepository(),
                new FakeUnitOfWork(),
                employee,
                credential,
                accessPoint);
    }

    [TestMethod]
    public async Task GetEmployeesListUseCase_ShouldReturnAllEmployees()
    {
        var (employees, _, _, _, _, _, _, _) = CreateAccessTestData();
        var useCase = new GetEmployeesListUseCase(employees);

        var result = await useCase.Handle(new GetEmployeesListQuery());

        Assert.AreEqual(1, result.TotalCount);
        Assert.AreEqual("Daniel", result.Items[0].Names);
        Assert.AreEqual(EmploymentStatus.Active, result.Items[0].Status);
    }

    [TestMethod]
    public async Task GetEmployeeByIdUseCase_NonExistingId_ShouldReturnNull()
    {
        var (employees, _, _, _, _, _, _, _) = CreateAccessTestData();
        var useCase = new GetEmployeeByIdUseCase(employees);

        var result = await useCase.Handle(new GetEmployeeByIdQuery(Guid.NewGuid()));

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task CreateEmployeeUseCase_WithDuplicatedIdentification_ShouldThrowException()
    {
        var (employees, _, _, _, unitOfWork, _, _, _) = CreateAccessTestData();
        var useCase = new CreateEmployeeUseCase(employees, unitOfWork);

        var command = new CreateEmployeeCommand
        {
            IdentificationNumber = "1020304050",
            Names = "Otro",
            LastNames = "Empleado",
            Email = "otro@logistics.com",
            Position = "Mensajero",
            Area = OrganizationalArea.Operations
        };

        await Assert.ThrowsExactlyAsync<BussinesRuleException>(() => useCase.Handle(command));
    }

    [TestMethod]
    public async Task CreateEmployeeUseCase_WithNewIdentification_ShouldReturnNewId()
    {
        var (employees, _, _, _, unitOfWork, _, _, _) = CreateAccessTestData();
        var useCase = new CreateEmployeeUseCase(employees, unitOfWork);

        var command = new CreateEmployeeCommand
        {
            IdentificationNumber = "5566778899",
            Names = "María",
            LastNames = "Gómez",
            Email = "maria.gomez@logistics.com",
            Position = "Auxiliar administrativa",
            Area = OrganizationalArea.Administration
        };

        var result = await useCase.Handle(command);

        Assert.AreNotEqual(Guid.Empty, result);
        Assert.AreEqual(1, unitOfWork.CommitCount);
    }

    [TestMethod]
    public async Task RegisterAccessUseCase_WithValidConditions_ShouldAuthorizeAndRecord()
    {
        var (employeesRepo, credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork, employee, credential, accessPoint) = CreateAccessTestData();
        var useCase = new RegisterAccessUseCase(credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork);

        var result = await useCase.Handle(new RegisterAccessCommand
        {
            CredentialCode = "crd-0001",
            AccessPointId = accessPoint.Id,
            MovementType = MovementType.Entry
        });

        Assert.IsTrue(result.Registered);
        Assert.AreEqual(AccessResult.Authorized, result.Result);
        Assert.IsNull(result.DeniedReason);
        Assert.AreEqual(1, recordsRepo.CreatedRecords.Count);
        Assert.AreEqual(1, unitOfWork.CommitCount);
    }

    [TestMethod]
    public async Task RegisterAccessUseCase_WithInactiveEmployee_ShouldDenyAndRecord()
    {
        var (_, credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork, employee, credential, accessPoint) = CreateAccessTestData();
        employee.Deactivate();
        var useCase = new RegisterAccessUseCase(credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork);

        var result = await useCase.Handle(new RegisterAccessCommand
        {
            CredentialCode = credential.Code,
            AccessPointId = accessPoint.Id,
            MovementType = MovementType.Entry
        });

        Assert.IsTrue(result.Registered);
        Assert.AreEqual(AccessResult.Denied, result.Result);
        Assert.AreEqual(DenialReason.EmployeeInactive, result.DeniedReason);
        Assert.AreEqual(1, recordsRepo.CreatedRecords.Count);
    }

    [TestMethod]
    public async Task RegisterAccessUseCase_WithBlockedCredential_ShouldDenyAndRecord()
    {
        var (_, credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork, employee, credential, accessPoint) = CreateAccessTestData();
        credential.Block();
        var useCase = new RegisterAccessUseCase(credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork);

        var result = await useCase.Handle(new RegisterAccessCommand
        {
            CredentialCode = credential.Code,
            AccessPointId = accessPoint.Id,
            MovementType = MovementType.Exit
        });

        Assert.AreEqual(AccessResult.Denied, result.Result);
        Assert.AreEqual(DenialReason.CredentialBlocked, result.DeniedReason);
    }

    [TestMethod]
    public async Task RegisterAccessUseCase_WithDisabledAccessPoint_ShouldDenyAndRecord()
    {
        var (_, credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork, employee, credential, accessPoint) = CreateAccessTestData();
        accessPoint.Disable();
        var useCase = new RegisterAccessUseCase(credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork);

        var result = await useCase.Handle(new RegisterAccessCommand
        {
            CredentialCode = credential.Code,
            AccessPointId = accessPoint.Id,
            MovementType = MovementType.Entry
        });

        Assert.AreEqual(AccessResult.Denied, result.Result);
        Assert.AreEqual(DenialReason.AccessPointDisabled, result.DeniedReason);
    }

    [TestMethod]
    public async Task RegisterAccessUseCase_WithUnknownCredential_ShouldNotRecord()
    {
        var (_, credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork, employee, credential, accessPoint) = CreateAccessTestData();
        var useCase = new RegisterAccessUseCase(credentialsRepo, accessPointsRepo, recordsRepo, unitOfWork);

        var result = await useCase.Handle(new RegisterAccessCommand
        {
            CredentialCode = "CRD-9999",
            AccessPointId = accessPoint.Id,
            MovementType = MovementType.Entry
        });

        Assert.IsFalse(result.Registered);
        Assert.AreEqual(0, recordsRepo.CreatedRecords.Count);
        Assert.AreEqual(0, unitOfWork.CommitCount);
    }

    [TestMethod]
    public async Task SimpleMediator_ShouldResolveAndExecuteUseCase()
    {
        var (employees, _, _, _, _, _, _, _) = CreateAccessTestData();

        var services = new ServiceCollection();
        services.AddScoped<IEmployeesRepository>(_ => employees);
        services.AddScoped<IMediator, SimpleMediator>();
        services.AddScoped<IRequestHandler<GetEmployeesListQuery, PaginationResponse<EmployeeListItemDTO>>, GetEmployeesListUseCase>();

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetEmployeesListQuery());

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.TotalCount);
    }
}

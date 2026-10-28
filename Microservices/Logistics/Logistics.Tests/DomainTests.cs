using Logistics.Domain.Common.ValueObjects;
using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Entities.Employees;
using Logistics.Domain.Exceptions;

namespace Logistics.Tests;

[TestClass]
public sealed class DomainTests
{
    [TestMethod]
    public void CreateEmployee_WithValidData_ShouldSucceed()
    {
        var email = Email.Create("daniel.zapata@logistics.com");

        var employee = new Employee("1020304050", "Daniel", "Zapata", email, "Operario de bodega", OrganizationalArea.Warehouse);

        Assert.AreNotEqual(Guid.Empty, employee.Id);
        Assert.AreEqual("1020304050", employee.IdentificationNumber);
        Assert.AreEqual("Daniel", employee.Names);
        Assert.AreEqual("Zapata", employee.LastNames);
        Assert.AreEqual(email, employee.Email);
        Assert.AreEqual("Operario de bodega", employee.Position);
        Assert.AreEqual(OrganizationalArea.Warehouse, employee.Area);
        Assert.AreEqual(EmploymentStatus.Active, employee.Status);
    }

    [TestMethod]
    public void CreateEmployee_WithEmptyIdentificationNumber_ShouldThrowException()
    {
        var email = Email.Create("daniel.zapata@logistics.com");

        Assert.ThrowsExactly<BussinesRuleException>(() => new Employee("", "Daniel", "Zapata", email, "Operario de bodega", OrganizationalArea.Warehouse));
    }

    [TestMethod]
    public void CreateEmployee_WithInvalidIdentificationNumberFormat_ShouldThrowException()
    {
        var email = Email.Create("daniel.zapata@logistics.com");

        Assert.ThrowsExactly<BussinesRuleException>(() => new Employee("CC 1020304050", "Daniel", "Zapata", email, "Operario de bodega", OrganizationalArea.Warehouse));
    }

    [TestMethod]
    public void UpdateEmployee_WithValidData_ShouldChangeData()
    {
        var employee = new Employee("1020304050", "Daniel", "Zapata", Email.Create("daniel.zapata@logistics.com"), "Operario de bodega", OrganizationalArea.Warehouse);
        var newEmail = Email.Create("daniel.nuevo@logistics.com");

        employee.Update("Daniel José", "Zapata Ríos", newEmail, "Coordinador de bodega", OrganizationalArea.Operations);

        Assert.AreEqual("Daniel José", employee.Names);
        Assert.AreEqual("Zapata Ríos", employee.LastNames);
        Assert.AreEqual(newEmail, employee.Email);
        Assert.AreEqual("Coordinador de bodega", employee.Position);
        Assert.AreEqual(OrganizationalArea.Operations, employee.Area);
        Assert.IsNotNull(employee.UpdatedAt);
    }

    [TestMethod]
    public void DeactivateEmployee_WhenActive_ShouldChangeStatusToInactive()
    {
        var employee = new Employee("1020304050", "Daniel", "Zapata", Email.Create("daniel.zapata@logistics.com"), "Operario de bodega", OrganizationalArea.Warehouse);

        employee.Deactivate();

        Assert.AreEqual(EmploymentStatus.Inactive, employee.Status);
    }

    [TestMethod]
    public void DeactivateEmployee_WhenAlreadyInactive_ShouldThrowException()
    {
        var employee = new Employee("1020304050", "Daniel", "Zapata", Email.Create("daniel.zapata@logistics.com"), "Operario de bodega", OrganizationalArea.Warehouse);
        employee.Deactivate();

        Assert.ThrowsExactly<BussinesRuleException>(() => employee.Deactivate());
    }

    [TestMethod]
    public void CreateEmail_WithValidAddress_ShouldSucceed()
    {
        var email = Email.Create("usuario@empresa.com.co");

        Assert.AreEqual("usuario@empresa.com.co", email.Value);
    }

    [TestMethod]
    public void CreateEmail_WithInvalidAddress_ShouldThrowException()
    {
        Assert.ThrowsExactly<BussinesRuleException>(() => Email.Create("usuario.empresa.com"));
    }

    [TestMethod]
    public void CreateCredential_WithValidData_ShouldStartActive()
    {
        var employeeId = Guid.NewGuid();

        var credential = new Credential("crd-0001", employeeId);

        Assert.AreNotEqual(Guid.Empty, credential.Id);
        Assert.AreEqual("CRD-0001", credential.Code);
        Assert.AreEqual(CredentialStatus.Active, credential.Status);
        Assert.AreEqual(employeeId, credential.EmployeeId);
    }

    [TestMethod]
    public void CreateCredential_WithEmptyEmployeeId_ShouldThrowException()
    {
        Assert.ThrowsExactly<BussinesRuleException>(() => new Credential("CRD-0001", Guid.Empty));
    }

    [TestMethod]
    public void BlockCredential_WhenActive_ShouldChangeStatusToBlocked()
    {
        var credential = new Credential("CRD-0001", Guid.NewGuid());

        credential.Block();

        Assert.AreEqual(CredentialStatus.Blocked, credential.Status);
        Assert.IsNotNull(credential.StatusChangedAt);
    }

    [TestMethod]
    public void BlockCredential_WhenRevoked_ShouldThrowException()
    {
        var credential = new Credential("CRD-0001", Guid.NewGuid());
        credential.Revoke();

        Assert.ThrowsExactly<BussinesRuleException>(() => credential.Block());
    }

    [TestMethod]
    public void RevokeCredential_WhenBlocked_ShouldChangeStatusToRevoked()
    {
        var credential = new Credential("CRD-0001", Guid.NewGuid());
        credential.Block();

        credential.Revoke();

        Assert.AreEqual(CredentialStatus.Revoked, credential.Status);
    }

    [TestMethod]
    public void CreateAccessPoint_WithValidData_ShouldBeEnabled()
    {
        var accessPoint = new AccessPoint("Recepción principal", AccessPointType.Reception, "Acceso peatonal de la sede principal");

        Assert.AreNotEqual(Guid.Empty, accessPoint.Id);
        Assert.AreEqual("Recepción principal", accessPoint.Name);
        Assert.AreEqual(AccessPointType.Reception, accessPoint.Type);
        Assert.IsTrue(accessPoint.IsEnabled);
    }

    [TestMethod]
    public void CreateAccessPoint_WithShortName_ShouldThrowException()
    {
        Assert.ThrowsExactly<BussinesRuleException>(() => new AccessPoint("R", AccessPointType.Reception));
    }

    [TestMethod]
    public void DisableAccessPoint_ThenEnable_ShouldToggleIsEnabled()
    {
        var accessPoint = new AccessPoint("Bodega 1", AccessPointType.Warehouse);

        accessPoint.Disable();
        Assert.IsFalse(accessPoint.IsEnabled);

        accessPoint.Enable();
        Assert.IsTrue(accessPoint.IsEnabled);
    }

    [TestMethod]
    public void CreateAuthorizedAccessRecord_WithValidData_ShouldSucceed()
    {
        var credentialId = Guid.NewGuid();
        var accessPointId = Guid.NewGuid();

        var record = AccessRecord.CreateAuthorized(credentialId, accessPointId, MovementType.Entry);

        Assert.AreNotEqual(Guid.Empty, record.Id);
        Assert.AreEqual(credentialId, record.CredentialId);
        Assert.AreEqual(accessPointId, record.AccessPointId);
        Assert.AreEqual(MovementType.Entry, record.MovementType);
        Assert.AreEqual(AccessResult.Authorized, record.Result);
        Assert.IsNull(record.DeniedReason);
    }

    [TestMethod]
    public void CreateDeniedAccessRecord_WithReason_ShouldSetDeniedReason()
    {
        var record = AccessRecord.CreateDenied(Guid.NewGuid(), Guid.NewGuid(), MovementType.Exit, DenialReason.CredentialBlocked);

        Assert.AreEqual(AccessResult.Denied, record.Result);
        Assert.AreEqual(DenialReason.CredentialBlocked, record.DeniedReason);
    }

    [TestMethod]
    public void CreateDeniedAccessRecord_WithoutReason_ShouldThrowException()
    {
        Assert.ThrowsExactly<BussinesRuleException>(() => new AccessRecord(Guid.NewGuid(), Guid.NewGuid(), MovementType.Entry, AccessResult.Denied));
    }

    [TestMethod]
    public void CreateAuthorizedAccessRecord_WithReason_ShouldThrowException()
    {
        Assert.ThrowsExactly<BussinesRuleException>(() => new AccessRecord(Guid.NewGuid(), Guid.NewGuid(), MovementType.Entry, AccessResult.Authorized, DenialReason.CredentialBlocked));
    }
}

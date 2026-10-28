using Logistics.Api.Controllers;
using Logistics.Application.UseCases.AccessPoints.Commands.CreateAccessPoint;
using Logistics.Application.UseCases.AccessPoints.Commands.UpdateAccessPointStatus;
using Logistics.Application.UseCases.AccessPoints.Queries.GetAccessPointsList;
using Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess;
using Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory;
using Logistics.Application.UseCases.Credentials.Commands.AssignCredential;
using Logistics.Application.UseCases.Credentials.Commands.BlockCredential;
using Logistics.Application.UseCases.Credentials.Commands.RevokeCredential;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialByCode;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee;
using Logistics.Application.UseCases.Employees.Commands.CreateEmployee;
using Logistics.Application.UseCases.Employees.Commands.DeactivateEmployee;
using Logistics.Application.UseCases.Employees.Commands.UpdateEmployee;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeeById;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeesList;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Entities.AccessRecords;
using Logistics.Domain.Entities.Credentials;
using Logistics.Domain.Entities.Employees;
using Logistics.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Tests;

[TestClass]
public sealed class ControllerTests
{
    private class FakeMediator : IMediator
    {
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            if (request is GetEmployeesListQuery)
            {
                var items = new List<EmployeeListItemDTO>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        IdentificationNumber = "1020304050",
                        Names = "Daniel",
                        LastNames = "Zapata",
                        Email = "daniel.zapata@logistics.com",
                        Position = "Operario de Bodega",
                        Area = OrganizationalArea.Warehouse,
                        Status = EmploymentStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                var response = PaginationResponse<EmployeeListItemDTO>.Create(items, items.Count, new PaginationRequest(1, 15));
                return Task.FromResult((TResponse)(object)response);
            }

            if (request is GetEmployeeByIdQuery byIdQuery)
            {
                if (byIdQuery.Id == Guid.Empty)
                {
                    return Task.FromResult((TResponse)(object)null!);
                }

                var details = new EmployeeDetailsDTO
                {
                    Id = byIdQuery.Id,
                    IdentificationNumber = "1020304050",
                    Names = "Daniel",
                    LastNames = "Zapata",
                    Email = "daniel.zapata@logistics.com",
                    Position = "Operario de Bodega",
                    Area = OrganizationalArea.Warehouse,
                    Status = EmploymentStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };
                return Task.FromResult((TResponse)(object)details);
            }

            if (request is CreateEmployeeCommand createCommand)
            {
                if (createCommand.IdentificationNumber == "1020304050")
                {
                    throw new BussinesRuleException("Ya existe un empleado con la identificación '1020304050'.");
                }

                return Task.FromResult((TResponse)(object)Guid.NewGuid());
            }

            if (request is DeactivateEmployeeCommand deactiveCommand)
            {
                return Task.FromResult((TResponse)(object)(deactiveCommand.Id != Guid.Empty));
            }

            if (request is RegisterAccessCommand accessCommand)
            {
                if (accessCommand.CredentialCode == "CRD-9999")
                {
                    return Task.FromResult((TResponse)(object)new RegisterAccessResultDTO
                    {
                        Registered = false,
                        Message = "No existe una credencial con el código 'CRD-9999'."
                    });
                }

                bool authorized = accessCommand.CredentialCode == "CRD-0001";
                return Task.FromResult((TResponse)(object)new RegisterAccessResultDTO
                {
                    Registered = true,
                    RecordId = Guid.NewGuid(),
                    Result = authorized ? AccessResult.Authorized : AccessResult.Denied,
                    DeniedReason = authorized ? null : DenialReason.CredentialBlocked,
                    Message = authorized ? "Acceso autorizado." : "Acceso rechazado: la credencial se encuentra bloqueada.",
                    OccurredAt = DateTime.UtcNow
                });
            }

            if (request is GetAccessHistoryQuery historyQuery)
            {
                var items = new List<AccessRecordDTO>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        CredentialId = Guid.NewGuid(),
                        CredentialCode = "CRD-0005",
                        EmployeeId = Guid.NewGuid(),
                        EmployeeName = "Andrés Molina",
                        AccessPointId = Guid.NewGuid(),
                        AccessPointName = "Bodega Central",
                        MovementType = MovementType.Entry,
                        Result = AccessResult.Denied,
                        DeniedReason = DenialReason.CredentialBlocked,
                        OccurredAt = DateTime.UtcNow
                    }
                };
                var response = PaginationResponse<AccessRecordDTO>.Create(items, items.Count, historyQuery.Pagination);
                return Task.FromResult((TResponse)(object)response);
            }

            if (request is UpdateEmployeeCommand updateCommand)
            {
                if (updateCommand.Email == "correo-invalido")
                {
                    throw new BussinesRuleException("El formato del correo electrónico no es válido.");
                }

                return Task.FromResult((TResponse)(object)(updateCommand.Id != Guid.Empty));
            }

            if (request is GetCredentialByCodeQuery credentialQuery)
            {
                if (credentialQuery.Code == "CRD-9999")
                {
                    return Task.FromResult((TResponse)(object)null!);
                }

                var credential = new CredentialDTO
                {
                    Id = Guid.NewGuid(),
                    Code = credentialQuery.Code,
                    Status = CredentialStatus.Active,
                    EmployeeId = Guid.NewGuid(),
                    EmployeeName = "Daniel Zapata",
                    AssignedAt = DateTime.UtcNow
                };
                return Task.FromResult((TResponse)(object)credential);
            }

            if (request is GetCredentialsByEmployeeQuery credentialsQuery)
            {
                var items = new List<CredentialDTO>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Code = "CRD-0001",
                        Status = CredentialStatus.Active,
                        EmployeeId = credentialsQuery.EmployeeId,
                        EmployeeName = "Daniel Zapata",
                        AssignedAt = DateTime.UtcNow
                    }
                };
                var response = PaginationResponse<CredentialDTO>.Create(items, items.Count, credentialsQuery.Pagination);
                return Task.FromResult((TResponse)(object)response);
            }

            if (request is AssignCredentialCommand assignCommand)
            {
                if (assignCommand.Code == "CRD-0001")
                {
                    throw new BussinesRuleException("Ya existe una credencial con el código 'CRD-0001'.");
                }

                return Task.FromResult((TResponse)(object)Guid.NewGuid());
            }

            if (request is BlockCredentialCommand blockCommand)
            {
                return Task.FromResult((TResponse)(object)(blockCommand.CredentialId != Guid.Empty));
            }

            if (request is RevokeCredentialCommand revokeCommand)
            {
                return Task.FromResult((TResponse)(object)(revokeCommand.CredentialId != Guid.Empty));
            }

            if (request is GetAccessPointsListQuery)
            {
                var items = new List<AccessPointDTO>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Recepción Principal",
                        Description = "Ingreso peatonal de la sede principal.",
                        Type = AccessPointType.Reception,
                        IsEnabled = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                var response = PaginationResponse<AccessPointDTO>.Create(items, items.Count, new PaginationRequest(1, 15));
                return Task.FromResult((TResponse)(object)response);
            }

            if (request is CreateAccessPointCommand createPointCommand)
            {
                if (createPointCommand.Name == "X")
                {
                    throw new BussinesRuleException("El nombre del punto de acceso debe tener al menos 2 caracteres.");
                }

                return Task.FromResult((TResponse)(object)Guid.NewGuid());
            }

            if (request is UpdateAccessPointStatusCommand statusCommand)
            {
                return Task.FromResult((TResponse)(object)(statusCommand.AccessPointId != Guid.Empty));
            }

            throw new NotImplementedException();
        }

        public Task Send(IRequest request) => Task.CompletedTask;
    }

    [TestMethod]
    public async Task GetEmployeesList_ShouldReturn200WithPaginationResponse()
    {
        var controller = new EmployeesController(new FakeMediator());

        var actionResult = await controller.GetList(1, 10);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);

        var payload = statusResult.Value as PaginationResponse<EmployeeListItemDTO>;
        Assert.IsNotNull(payload);
        Assert.AreEqual(1, payload.TotalCount);
        Assert.AreEqual("Daniel", payload.Items[0].Names);
    }

    [TestMethod]
    public async Task GetEmployeeById_WhenNotFound_ShouldReturn404()
    {
        var controller = new EmployeesController(new FakeMediator());

        var actionResult = await controller.GetById(Guid.Empty);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task CreateEmployee_WithValidData_ShouldReturn201()
    {
        var controller = new EmployeesController(new FakeMediator());

        var command = new CreateEmployeeCommand
        {
            IdentificationNumber = "9988776655",
            Names = "Sofía",
            LastNames = "Torres",
            Email = "sofia.torres@logistics.com",
            Position = "Analista de Inventarios",
            Area = OrganizationalArea.Warehouse
        };

        var actionResult = await controller.Create(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status201Created, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task CreateEmployee_WithDuplicatedIdentification_ShouldReturn400()
    {
        var controller = new EmployeesController(new FakeMediator());

        var command = new CreateEmployeeCommand
        {
            IdentificationNumber = "1020304050",
            Names = "Duplicado",
            LastNames = "Empleado",
            Email = "duplicado@logistics.com",
            Position = "Operario",
            Area = OrganizationalArea.Warehouse
        };

        var actionResult = await controller.Create(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status400BadRequest, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task DeactivateEmployee_WhenNotFound_ShouldReturn404()
    {
        var controller = new EmployeesController(new FakeMediator());

        var actionResult = await controller.Deactivate(Guid.Empty);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task RegisterAccess_WhenAuthorized_ShouldReturn200()
    {
        var controller = new AccessController(new FakeMediator());

        var command = new RegisterAccessCommand
        {
            CredentialCode = "CRD-0001",
            AccessPointId = Guid.NewGuid(),
            MovementType = MovementType.Entry
        };

        var actionResult = await controller.Register(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task RegisterAccess_WhenDenied_ShouldReturn403()
    {
        var controller = new AccessController(new FakeMediator());

        var command = new RegisterAccessCommand
        {
            CredentialCode = "CRD-0005",
            AccessPointId = Guid.NewGuid(),
            MovementType = MovementType.Entry
        };

        var actionResult = await controller.Register(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status403Forbidden, statusResult.StatusCode);

        var payload = statusResult.Value as RegisterAccessResultDTO;
        Assert.IsNotNull(payload);
        Assert.AreEqual(DenialReason.CredentialBlocked, payload.DeniedReason);
    }

    [TestMethod]
    public async Task RegisterAccess_WithUnknownCredential_ShouldReturn404()
    {
        var controller = new AccessController(new FakeMediator());

        var command = new RegisterAccessCommand
        {
            CredentialCode = "CRD-9999",
            AccessPointId = Guid.NewGuid(),
            MovementType = MovementType.Entry
        };

        var actionResult = await controller.Register(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task GetAccessHistory_ShouldReturn200WithRecords()
    {
        var controller = new AccessController(new FakeMediator());

        var actionResult = await controller.GetHistory(1, 10);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);

        var payload = statusResult.Value as PaginationResponse<AccessRecordDTO>;
        Assert.IsNotNull(payload);
        Assert.AreEqual(1, payload.TotalCount);
        Assert.AreEqual(AccessResult.Denied, payload.Items[0].Result);
    }

    [TestMethod]
    public async Task UpdateEmployee_WhenFound_ShouldReturn200()
    {
        var controller = new EmployeesController(new FakeMediator());
        var command = new UpdateEmployeeCommand { Names = "Daniel", LastNames = "Zapata", Email = "daniel@logistics.com", Position = "Operario", Area = OrganizationalArea.Warehouse };

        var actionResult = await controller.Update(Guid.NewGuid(), command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task UpdateEmployee_WhenNotFound_ShouldReturn404()
    {
        var controller = new EmployeesController(new FakeMediator());
        var command = new UpdateEmployeeCommand { Names = "Daniel", LastNames = "Zapata", Email = "daniel@logistics.com", Position = "Operario", Area = OrganizationalArea.Warehouse };

        var actionResult = await controller.Update(Guid.Empty, command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task UpdateEmployee_WithInvalidEmail_ShouldReturn400()
    {
        var controller = new EmployeesController(new FakeMediator());
        var command = new UpdateEmployeeCommand { Names = "Daniel", LastNames = "Zapata", Email = "correo-invalido", Position = "Operario", Area = OrganizationalArea.Warehouse };

        var actionResult = await controller.Update(Guid.NewGuid(), command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status400BadRequest, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task DeactivateEmployee_WhenFound_ShouldReturn200()
    {
        var controller = new EmployeesController(new FakeMediator());

        var actionResult = await controller.Deactivate(Guid.NewGuid());
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task GetCredentialByCode_WhenFound_ShouldReturn200()
    {
        var controller = new CredentialsController(new FakeMediator());

        var actionResult = await controller.GetByCode("CRD-0001");
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);

        var payload = statusResult.Value as CredentialDTO;
        Assert.IsNotNull(payload);
        Assert.AreEqual("CRD-0001", payload.Code);
    }

    [TestMethod]
    public async Task GetCredentialByCode_WhenNotFound_ShouldReturn404()
    {
        var controller = new CredentialsController(new FakeMediator());

        var actionResult = await controller.GetByCode("CRD-9999");
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task GetCredentialsByEmployee_ShouldReturn200()
    {
        var controller = new CredentialsController(new FakeMediator());

        var actionResult = await controller.GetByEmployee(Guid.NewGuid(), 1, 10);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);

        var payload = statusResult.Value as PaginationResponse<CredentialDTO>;
        Assert.IsNotNull(payload);
        Assert.AreEqual(1, payload.TotalCount);
    }

    [TestMethod]
    public async Task AssignCredential_WithValidData_ShouldReturn201()
    {
        var controller = new CredentialsController(new FakeMediator());
        var command = new AssignCredentialCommand { Code = "CRD-0007", EmployeeId = Guid.NewGuid() };

        var actionResult = await controller.Assign(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status201Created, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task AssignCredential_WithDuplicatedCode_ShouldReturn400()
    {
        var controller = new CredentialsController(new FakeMediator());
        var command = new AssignCredentialCommand { Code = "CRD-0001", EmployeeId = Guid.NewGuid() };

        var actionResult = await controller.Assign(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status400BadRequest, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task BlockCredential_WhenFound_ShouldReturn200()
    {
        var controller = new CredentialsController(new FakeMediator());

        var actionResult = await controller.Block(Guid.NewGuid());
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task BlockCredential_WhenNotFound_ShouldReturn404()
    {
        var controller = new CredentialsController(new FakeMediator());

        var actionResult = await controller.Block(Guid.Empty);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task RevokeCredential_WhenFound_ShouldReturn200()
    {
        var controller = new CredentialsController(new FakeMediator());

        var actionResult = await controller.Revoke(Guid.NewGuid());
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task GetAccessPointsList_ShouldReturn200()
    {
        var controller = new AccessPointsController(new FakeMediator());

        var actionResult = await controller.GetList(1, 10);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);

        var payload = statusResult.Value as PaginationResponse<AccessPointDTO>;
        Assert.IsNotNull(payload);
        Assert.AreEqual(1, payload.TotalCount);
        Assert.IsTrue(payload.Items[0].IsEnabled);
    }

    [TestMethod]
    public async Task CreateAccessPoint_WithValidData_ShouldReturn201()
    {
        var controller = new AccessPointsController(new FakeMediator());
        var command = new CreateAccessPointCommand { Name = "Bodega Sur", Type = AccessPointType.Warehouse, Description = "Acceso sur" };

        var actionResult = await controller.Create(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status201Created, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task CreateAccessPoint_WithInvalidName_ShouldReturn400()
    {
        var controller = new AccessPointsController(new FakeMediator());
        var command = new CreateAccessPointCommand { Name = "X", Type = AccessPointType.Warehouse };

        var actionResult = await controller.Create(command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status400BadRequest, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task UpdateAccessPointStatus_WhenFound_ShouldReturn200()
    {
        var controller = new AccessPointsController(new FakeMediator());
        var command = new UpdateAccessPointStatusCommand(Guid.NewGuid(), false);

        var actionResult = await controller.UpdateStatus(Guid.NewGuid(), command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status200OK, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task UpdateAccessPointStatus_WhenNotFound_ShouldReturn404()
    {
        var controller = new AccessPointsController(new FakeMediator());
        var command = new UpdateAccessPointStatusCommand(Guid.Empty, true);

        var actionResult = await controller.UpdateStatus(Guid.Empty, command);
        var statusResult = actionResult as ObjectResult;

        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, statusResult.StatusCode);
    }
}

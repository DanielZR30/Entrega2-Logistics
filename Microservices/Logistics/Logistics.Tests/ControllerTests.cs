using Logistics.Api.Controllers;
using Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess;
using Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory;
using Logistics.Application.UseCases.Employees.Commands.CreateEmployee;
using Logistics.Application.UseCases.Employees.Commands.DeactivateEmployee;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeeById;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeesList;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessRecords;
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
}

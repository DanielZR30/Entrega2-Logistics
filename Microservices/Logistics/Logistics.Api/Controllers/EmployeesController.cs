using Logistics.Application.UseCases.Employees.Commands.CreateEmployee;
using Logistics.Application.UseCases.Employees.Commands.DeactivateEmployee;
using Logistics.Application.UseCases.Employees.Commands.UpdateEmployee;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeeById;
using Logistics.Application.UseCases.Employees.Queries.GetEmployeesList;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.Employees;
using Logistics.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// RF-01: Consultar empleados con paginación y filtros opcionales por estado, área y búsqueda.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int pageNumber = 1,
                                                 [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
                                                 [FromQuery] EmploymentStatus? status = null,
                                                 [FromQuery] OrganizationalArea? area = null,
                                                 [FromQuery] string? searchTerm = null)
        {
            GetEmployeesListQuery query = new()
            {
                Pagination = new PaginationRequest(pageNumber, pageSize),
                Status = status,
                Area = area,
                SearchTerm = searchTerm
            };

            PaginationResponse<EmployeeListItemDTO> result = await _mediator.Send(query);

            return StatusCode(StatusCodes.Status200OK, result);
        }

        /// <summary>
        /// RF-01: Consultar el detalle de un empleado por su identificador, incluyendo sus credenciales.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            GetEmployeeByIdQuery query = new(id);

            EmployeeDetailsDTO? result = await _mediator.Send(query);

            if (result is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = $"No se encontró un empleado con el identificador '{id}'." });
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        /// <summary>
        /// RF-01: Registrar un nuevo empleado con identificación única.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            try
            {
                Guid employeeId = await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new { id = employeeId, message = "Empleado registrado correctamente." });
            }
            catch (BussinesRuleException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF-01: Actualizar la información de un empleado existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEmployeeCommand command)
        {
            try
            {
                command.Id = id;
                bool updated = await _mediator.Send(command);

                if (!updated)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = $"No se encontró un empleado con el identificador '{id}'." });
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Empleado actualizado correctamente." });
            }
            catch (BussinesRuleException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF-01: Desactivar un empleado (el vínculo laboral pasa a inactivo).
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate([FromRoute] Guid id)
        {
            try
            {
                DeactivateEmployeeCommand command = new(id);
                bool deactivated = await _mediator.Send(command);

                if (!deactivated)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = $"No se encontró un empleado con el identificador '{id}'." });
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Empleado desactivado correctamente." });
            }
            catch (BussinesRuleException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }
        }
    }
}

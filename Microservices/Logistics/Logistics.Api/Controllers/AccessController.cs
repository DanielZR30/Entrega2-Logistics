using Logistics.Application.UseCases.AccessRecords.Commands.RegisterAccess;
using Logistics.Application.UseCases.AccessRecords.Queries.GetAccessHistory;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessRecords;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccessController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// RF-03: Registrar una solicitud de acceso (entrada o salida) validando las condiciones
        /// del empleado, la credencial y el punto de acceso. Los accesos rechazados también quedan registrados.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccessCommand command)
        {
            RegisterAccessResultDTO result = await _mediator.Send(command);

            if (!result.Registered)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = result.Message });
            }

            int statusCode = result.Result == AccessResult.Authorized
                ? StatusCodes.Status200OK
                : StatusCodes.Status403Forbidden;

            return StatusCode(statusCode, result);
        }

        /// <summary>
        /// RF-04: Consultar el historial de movimientos de acceso con filtros por empleado,
        /// credencial, punto de acceso, resultado y rango de fechas, con paginación.
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int pageNumber = 1,
                                                    [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
                                                    [FromQuery] Guid? employeeId = null,
                                                    [FromQuery] Guid? credentialId = null,
                                                    [FromQuery] Guid? accessPointId = null,
                                                    [FromQuery] AccessResult? result = null,
                                                    [FromQuery] DateTime? dateFrom = null,
                                                    [FromQuery] DateTime? dateTo = null)
        {
            GetAccessHistoryQuery query = new()
            {
                Pagination = new PaginationRequest(pageNumber, pageSize),
                EmployeeId = employeeId,
                CredentialId = credentialId,
                AccessPointId = accessPointId,
                Result = result,
                DateFrom = dateFrom,
                DateTo = dateTo
            };

            PaginationResponse<AccessRecordDTO> response = await _mediator.Send(query);

            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}

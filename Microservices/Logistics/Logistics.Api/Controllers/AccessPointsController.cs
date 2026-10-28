using Logistics.Application.UseCases.AccessPoints.Commands.CreateAccessPoint;
using Logistics.Application.UseCases.AccessPoints.Commands.UpdateAccessPointStatus;
using Logistics.Application.UseCases.AccessPoints.Queries.GetAccessPointsList;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Entities.AccessPoints;
using Logistics.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessPointsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccessPointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// RF-06: Consultar los puntos de acceso físicos con filtros por tipo y estado de habilitación.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int pageNumber = 1,
                                                 [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
                                                 [FromQuery] AccessPointType? type = null,
                                                 [FromQuery] bool? isEnabled = null)
        {
            GetAccessPointsListQuery query = new()
            {
                Pagination = new PaginationRequest(pageNumber, pageSize),
                Type = type,
                IsEnabled = isEnabled
            };

            PaginationResponse<AccessPointDTO> result = await _mediator.Send(query);

            return StatusCode(StatusCodes.Status200OK, result);
        }

        /// <summary>
        /// RF-06: Registrar un nuevo punto de acceso físico.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccessPointCommand command)
        {
            try
            {
                Guid accessPointId = await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new { id = accessPointId, message = "Punto de acceso registrado correctamente." });
            }
            catch (BussinesRuleException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF-06: Habilitar o deshabilitar un punto de acceso existente.
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateAccessPointStatusCommand command)
        {
            command.AccessPointId = id;

            bool updated = await _mediator.Send(command);

            if (!updated)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = $"No se encontró un punto de acceso con el identificador '{id}'." });
            }

            return StatusCode(StatusCodes.Status200OK, new { message = command.Enable ? "Punto de acceso habilitado correctamente." : "Punto de acceso deshabilitado correctamente." });
        }
    }
}

using Logistics.Application.UseCases.Credentials.Commands.AssignCredential;
using Logistics.Application.UseCases.Credentials.Commands.BlockCredential;
using Logistics.Application.UseCases.Credentials.Commands.RevokeCredential;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialByCode;
using Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee;
using Logistics.Application.Utilities.Mediator;
using Logistics.Application.Utilities.Pagination;
using Logistics.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CredentialsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// RF-02: Consultar una credencial y su estado a partir de su código único.
        /// </summary>
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode([FromRoute] string code)
        {
            GetCredentialByCodeQuery query = new(code);

            CredentialDTO? result = await _mediator.Send(query);

            if (result is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = $"No se encontró una credencial con el código '{code}'." });
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        /// <summary>
        /// RF-02: Consultar las credenciales asignadas a un empleado con paginación.
        /// </summary>
        [HttpGet("by-employee/{employeeId:guid}")]
        public async Task<IActionResult> GetByEmployee([FromRoute] Guid employeeId,
                                                       [FromQuery] int pageNumber = 1,
                                                       [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE)
        {
            GetCredentialsByEmployeeQuery query = new(employeeId, new PaginationRequest(pageNumber, pageSize));

            PaginationResponse<CredentialDTO> result = await _mediator.Send(query);

            return StatusCode(StatusCodes.Status200OK, result);
        }

        /// <summary>
        /// RF-02: Asignar una credencial de acceso con código único a un empleado.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] AssignCredentialCommand command)
        {
            try
            {
                Guid credentialId = await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new { id = credentialId, message = "Credencial asignada correctamente." });
            }
            catch (BussinesRuleException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF-02: Bloquear una credencial vigente.
        /// </summary>
        [HttpPatch("{id:guid}/block")]
        public async Task<IActionResult> Block([FromRoute] Guid id)
        {
            try
            {
                BlockCredentialCommand command = new(id);
                bool blocked = await _mediator.Send(command);

                if (!blocked)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = $"No se encontró una credencial con el identificador '{id}'." });
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Credencial bloqueada correctamente." });
            }
            catch (BussinesRuleException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF-02: Revocar definitivamente una credencial.
        /// </summary>
        [HttpPatch("{id:guid}/revoke")]
        public async Task<IActionResult> Revoke([FromRoute] Guid id)
        {
            try
            {
                RevokeCredentialCommand command = new(id);
                bool revoked = await _mediator.Send(command);

                if (!revoked)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = $"No se encontró una credencial con el identificador '{id}'." });
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Credencial revocada correctamente." });
            }
            catch (BussinesRuleException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }
        }
    }
}

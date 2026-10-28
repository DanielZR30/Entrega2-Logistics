using Logistics.Domain.Exceptions;

namespace Logistics.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BussinesRuleException ex)
            {
                _logger.LogWarning(ex, "Regla de negocio violada en {Ruta}.", context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado en {Ruta}.", context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = "Ocurrió un error inesperado al procesar la solicitud." });
            }
        }
    }
}

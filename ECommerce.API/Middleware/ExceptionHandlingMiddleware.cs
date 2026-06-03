using ECommerce.Application.Common;
using ECommerce.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ECommerce.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                NotFoundException => new ErrorResponse((int)HttpStatusCode.NotFound, exception.Message),
                BadRequestException => new ErrorResponse((int)HttpStatusCode.BadRequest, exception.Message),
                _ => new ErrorResponse((int)HttpStatusCode.InternalServerError, "An internal server error has occurred.", _env.IsDevelopment() ? exception.StackTrace : null)
            };

            context.Response.StatusCode = response.StatusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);
        }
    }
}

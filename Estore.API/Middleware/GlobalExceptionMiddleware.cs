using Estore.Application.Common.GeneralResult;
using Estore.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Estore.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                BaseException baseEx => new ApiResponse
                {
                    Success = false,
                    Message = baseEx.Message,
                    Errors = new List<string> { baseEx.Message }
                },
                _ => new ApiResponse
                {
                    Success = false,
                    Message = "An internal server error occurred",
                    Errors = new List<string> { "Internal server error" }
                }
            };

            context.Response.StatusCode = exception switch
            {
                BaseException baseEx => baseEx.StatusCode,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}

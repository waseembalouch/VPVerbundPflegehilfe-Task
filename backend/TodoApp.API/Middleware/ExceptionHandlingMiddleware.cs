using System.Net;
using System.Text.Json;
using TodoApp.Domain.Exceptions;

namespace TodoApp.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                notFound.Message,
                (IDictionary<string, string[]>?)null),

            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "One or more validation errors occurred.",
                validation.Errors),

            _ => (
                HttpStatusCode.InternalServerError,
                "An error occurred processing your request.",
                (IDictionary<string, string[]>?)null)
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            message,
            errors
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

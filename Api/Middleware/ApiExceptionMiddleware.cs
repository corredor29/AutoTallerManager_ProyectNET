using Api.Responses;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid request"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Business rule violation"),
            _ => (HttpStatusCode.InternalServerError, "Unexpected error")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new ApiErrorResponse
        {
            Success = false,
            Title = title,
            Detail = exception.Message,
            StatusCode = (int)statusCode,
            TraceId = context.TraceIdentifier
        });

        return context.Response.WriteAsync(payload);
    }
}

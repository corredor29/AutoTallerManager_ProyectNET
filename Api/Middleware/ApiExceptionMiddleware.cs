using Api.Responses;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;

// Middleware global que captura excepciones no controladas y devuelve un error JSON uniforme.
public sealed class ApiExceptionMiddleware
{
    // Referencia al siguiente middleware del pipeline HTTP.
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Envuelve la ejecucion del siguiente middleware para interceptar cualquier excepcion.
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

    // Traduce excepciones conocidas a codigos HTTP y mensajes mas entendibles para el cliente.
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
            // Incluye el detalle original para ayudar a diagnosticar la causa del problema.
            Detail = exception.Message,
            StatusCode = (int)statusCode,
            TraceId = context.TraceIdentifier
        });

        return context.Response.WriteAsync(payload);
    }
}

using Api.Responses;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;

// Funcion para manejar las excepciones de manera global en la aplicación, 
//capturando cualquier excepción no manejada y devolviendo una respuesta 
//JSON con detalles del error.
public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)  // Método principal que se ejecuta para cada solicitud, 
                                                        // envolviendo la ejecución del siguiente middleware en un bloque try-catch para capturar cualquier excepción que ocurra durante el procesamiento de la solicitud.
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

    // Función para mapear diferentes tipos de excepciones a códigos de estado HTTP y mensajes de error personalizados.
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
            Detail = exception.Message,      // Proporciona detalles específicos del error para ayudar a entender la causa del problema.
            StatusCode = (int)statusCode,
            TraceId = context.TraceIdentifier
        });

        return context.Response.WriteAsync(payload);
    }
}

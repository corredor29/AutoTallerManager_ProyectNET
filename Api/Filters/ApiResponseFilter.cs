using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public sealed class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult &&
            objectResult.Value is not null &&
            objectResult.Value is not ApiSuccessResponse<object> &&
            objectResult.Value is not ApiErrorResponse &&
            objectResult.Value is not ProblemDetails)
        {
            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
            var traceId = context.HttpContext.TraceIdentifier;

            objectResult.Value = new ApiSuccessResponse<object>
            {
                StatusCode = statusCode,
                Message = GetMessageForStatusCode(statusCode),
                Data = objectResult.Value,
                TraceId = traceId
            };
        }

        await next();
    }

    private static string GetMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status200OK => "Request completed successfully.",
            StatusCodes.Status201Created => "Resource created successfully.",
            StatusCodes.Status202Accepted => "Request accepted successfully.",
            _ => "Request completed successfully."
        };
    }
}

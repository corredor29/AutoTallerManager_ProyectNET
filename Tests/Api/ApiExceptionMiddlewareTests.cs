using System.Text.Json;
using Api.Middleware;
using Api.Responses;
using Microsoft.AspNetCore.Http;

namespace AutoTallerManager.Tests.Api;

public sealed class ApiExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsStandardizedConflictResponse_WhenBusinessRuleFails()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ApiExceptionMiddleware(_ => throw new InvalidOperationException("Order already closed."));

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        var payload = await JsonSerializer.DeserializeAsync<ApiErrorResponse>(context.Response.Body);

        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(StatusCodes.Status409Conflict, payload.StatusCode);
        Assert.Equal("Business rule violation", payload.Title);
        Assert.Equal("Order already closed.", payload.Detail);
        Assert.False(string.IsNullOrWhiteSpace(payload.TraceId));
    }
}

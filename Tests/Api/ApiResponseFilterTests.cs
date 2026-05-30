using Api.Filters;
using Api.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace AutoTallerManager.Tests.Api;

public sealed class ApiResponseFilterTests
{
    [Fact]
    public async Task OnResultExecutionAsync_WrapsObjectResult_InSuccessEnvelope()
    {
        var filter = new ApiResponseFilter();
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var result = new OkObjectResult(new { Name = "Brake Pad" });
        var context = new ResultExecutingContext(
            actionContext,
            [],
            result,
            controller: new object());

        await filter.OnResultExecutionAsync(context, () =>
        {
            var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new object());
            return Task.FromResult(executedContext);
        });

        var wrapped = Assert.IsType<OkObjectResult>(context.Result);
        var payload = Assert.IsType<ApiSuccessResponse<object>>(wrapped.Value);
        Assert.True(payload.Success);
        Assert.Equal(StatusCodes.Status200OK, payload.StatusCode);
        Assert.Equal("Request completed successfully.", payload.Message);
        Assert.NotNull(payload.Data);
    }

    [Fact]
    public async Task OnResultExecutionAsync_DoesNotWrapProblemDetails()
    {
        var filter = new ApiResponseFilter();
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var problem = new ProblemDetails { Title = "Invalid request" };
        var result = new BadRequestObjectResult(problem);
        var context = new ResultExecutingContext(actionContext, [], result, controller: new object());

        await filter.OnResultExecutionAsync(context, () =>
        {
            var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new object());
            return Task.FromResult(executedContext);
        });

        var objectResult = Assert.IsType<BadRequestObjectResult>(context.Result);
        Assert.Same(problem, objectResult.Value);
    }
}

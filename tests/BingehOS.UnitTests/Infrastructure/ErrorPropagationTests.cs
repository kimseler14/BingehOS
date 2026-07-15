using BingehOS.Api.Filters;
using BingehOS.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace BingehOS.UnitTests.Infrastructure;

public class ErrorPropagationTests
{
    [Fact]
    public async Task Publish_WithoutChannel_Throws()
    {
        var configuration = new ConfigurationBuilder().Build();
        var publisher = new RabbitMqEventPublisher(
            configuration,
            NullLogger<RabbitMqEventPublisher>.Instance);

        await Assert.ThrowsAsync<EventPublishingException>(
            () => publisher.Publish(new TestEvent()));
    }

    [Fact]
    public void GlobalExceptionFilter_UnexpectedException_ReturnsInternalError()
    {
        var context = CreateExceptionContext(new Exception("sensitive details"));
        var filter = new GlobalExceptionFilter(NullLogger<GlobalExceptionFilter>.Instance);

        filter.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.True(context.ExceptionHandled);
        Assert.DoesNotContain("sensitive details", result.Value?.ToString());
    }

    [Fact]
    public void GlobalExceptionFilter_RequestCancellation_RemainsUnhandled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var context = CreateExceptionContext(new OperationCanceledException(cts.Token));
        context.HttpContext.RequestAborted = cts.Token;
        var filter = new GlobalExceptionFilter(NullLogger<GlobalExceptionFilter>.Instance);

        filter.OnException(context);

        Assert.Null(context.Result);
        Assert.False(context.ExceptionHandled);
    }

    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    private sealed record TestEvent;
}

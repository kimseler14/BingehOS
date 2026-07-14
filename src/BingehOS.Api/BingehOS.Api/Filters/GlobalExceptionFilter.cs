using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BingehOS.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext ctx)
    {
        if (ctx.Exception is OperationCanceledException &&
            ctx.HttpContext.RequestAborted.IsCancellationRequested)
            return;

        var (code, msg) = ctx.Exception switch
        {
            InvalidOperationException => (StatusCodes.Status400BadRequest, ctx.Exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, ctx.Exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, ctx.Exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal error")
        };

        if (code == StatusCodes.Status500InternalServerError)
            _logger.LogError(ctx.Exception, "Unhandled exception processing request");

        ctx.Result = new ObjectResult(new { success = false, error = msg })
        {
            StatusCode = code
        };
        ctx.ExceptionHandled = true;
    }
}

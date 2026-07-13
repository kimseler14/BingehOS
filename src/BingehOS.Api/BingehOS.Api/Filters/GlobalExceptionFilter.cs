using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BingehOS.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext ctx)
    {
        var (code, msg) = ctx.Exception switch
        {
            InvalidOperationException => (StatusCodes.Status400BadRequest, ctx.Exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, ctx.Exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, ctx.Exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal error")
        };
        ctx.Result = new ObjectResult(new { success = false, error = msg })
        {
            StatusCode = code
        };
        ctx.ExceptionHandled = true;
    }
}

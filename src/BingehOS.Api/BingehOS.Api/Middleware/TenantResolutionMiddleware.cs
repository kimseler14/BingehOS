namespace BingehOS.Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    public TenantResolutionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (ctx.Request.Headers.TryGetValue("x-tenant-id", out var v) &&
            Guid.TryParse(v.ToString(), out var tenantId))
        {
            ctx.Items["TenantId"] = tenantId;
        }
        await _next(ctx);
    }
}

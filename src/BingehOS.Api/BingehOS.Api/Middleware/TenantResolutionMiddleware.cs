namespace BingehOS.Api.Middleware;

public class TenantResolutionMiddleware
{
    public const string TenantClaimType = "tenant_id";

    private readonly RequestDelegate _next;
    public TenantResolutionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx)
    {
        // For authenticated requests the effective tenant is derived exclusively
        // from the JWT's "tenant_id" claim. The client-supplied x-tenant-id header
        // is deliberately ignored in this case so a user from tenant A cannot spoof
        // tenant B (horizontal privilege escalation). If the claim is missing we fall
        // back to the header for robustness, but the claim always wins when present.
        if (ctx.User.Identity is { IsAuthenticated: true } &&
            ctx.User.FindFirst(TenantClaimType) is { } claim &&
            Guid.TryParse(claim.Value, out var claimTenant))
        {
            ctx.Items["TenantId"] = claimTenant;
            await _next(ctx);
            return;
        }

        // Unauthenticated/bootstrap requests (e.g. login, tenant selection) resolve
        // the tenant from the x-tenant-id header.
        if (ctx.Request.Headers.TryGetValue("x-tenant-id", out var v) &&
            Guid.TryParse(v.ToString(), out var tenantId))
        {
            ctx.Items["TenantId"] = tenantId;
        }

        await _next(ctx);
    }
}

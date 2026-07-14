using BingehOS.Api.Middleware;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Xunit;

namespace BingehOS.UnitTests.Auth;

public class TenantResolutionMiddlewareTests
{
    private static TenantResolutionMiddleware CreateMiddleware(Action<HttpContext>? onNext = null)
    {
        RequestDelegate next = ctx =>
        {
            onNext?.Invoke(ctx);
            return Task.CompletedTask;
        };
        return new TenantResolutionMiddleware(next);
    }

    private static void SetUser(HttpContext ctx, bool authenticated, Guid? tenantClaim)
    {
        var claims = new List<Claim>();
        if (tenantClaim is not null)
            claims.Add(new Claim(TenantResolutionMiddleware.TenantClaimType, tenantClaim.ToString()!));

        var identity = new ClaimsIdentity(
            claims,
            authenticated ? "test-auth" : null);

        ctx.User = new ClaimsPrincipal(identity);
    }

    [Fact]
    public void Authenticated_ClaimWins_OverSpoofedHeader()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        var ctx = new DefaultHttpContext();
        SetUser(ctx, authenticated: true, tenantClaim: tenantA);
        ctx.Request.Headers["x-tenant-id"] = tenantB.ToString();

        var mw = CreateMiddleware();
        mw.InvokeAsync(ctx).GetAwaiter().GetResult();

        Assert.Equal(tenantA, ctx.Items["TenantId"]);
        Assert.NotEqual(tenantB, ctx.Items["TenantId"]);
    }

    [Fact]
    public void Unauthenticated_HeaderIsUsed()
    {
        var tenantB = Guid.NewGuid();

        var ctx = new DefaultHttpContext();
        SetUser(ctx, authenticated: false, tenantClaim: null);
        ctx.Request.Headers["x-tenant-id"] = tenantB.ToString();

        var mw = CreateMiddleware();
        mw.InvokeAsync(ctx).GetAwaiter().GetResult();

        Assert.Equal(tenantB, ctx.Items["TenantId"]);
    }

    [Fact]
    public void Authenticated_NoClaim_FallsBackToHeader()
    {
        var tenantB = Guid.NewGuid();

        var ctx = new DefaultHttpContext();
        SetUser(ctx, authenticated: true, tenantClaim: null);
        ctx.Request.Headers["x-tenant-id"] = tenantB.ToString();

        var mw = CreateMiddleware();
        mw.InvokeAsync(ctx).GetAwaiter().GetResult();

        Assert.Equal(tenantB, ctx.Items["TenantId"]);
    }
}

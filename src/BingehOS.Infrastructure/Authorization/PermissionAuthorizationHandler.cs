using System.IdentityModel.Tokens.Jwt;
using BingehOS.Infrastructure;
using BingehOS.Modules.Identity.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly AppDbContext _db;
    private readonly ITenantProvider _tenantProvider;

    public PermissionAuthorizationHandler(AppDbContext db, ITenantProvider tenantProvider)
    {
        _db = db;
        _tenantProvider = tenantProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User == null || !context.User.Identity?.IsAuthenticated == true)
        {
            context.Fail();
            return;
        }

        var tenantId = _tenantProvider.CurrentTenantId;
        if (tenantId == Guid.Empty)
        {
            context.Fail();
            return;
        }

        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        var hasPermission = await _db.Set<UserRole>()
            .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
            .SelectMany(ur => _db.Set<RolePermission>()
                .Where(rp => rp.RoleId == ur.RoleId && rp.TenantId == tenantId)
                .Select(rp => rp.Permission!.Name))
            .AnyAsync(name => name == requirement.PermissionName);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}

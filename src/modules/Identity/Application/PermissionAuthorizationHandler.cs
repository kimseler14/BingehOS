using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Linq;

namespace BingehOS.Modules.Identity.Application;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger) => _logger = logger;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissionClaim = context.User.FindFirst("permission");
        if (permissionClaim == null)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value;
            _logger.LogWarning("No permission claim found for user {UserId}", userId);
            return Task.CompletedTask;
        }

        var userPermissions = permissionClaim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (userPermissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
        else
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value;
            _logger.LogWarning("User {UserId} lacks permission {Permission}", userId, requirement.Permission);
        }

        return Task.CompletedTask;
    }
}

using Microsoft.AspNetCore.Authorization;

namespace BingehOS.Modules.Identity.Application;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}

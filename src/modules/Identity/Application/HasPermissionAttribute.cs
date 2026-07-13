using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BingehOS.Modules.Identity.Application;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base("HasPermission")
    {
        Permission = permission;
    }

    public string Permission { get; }
}

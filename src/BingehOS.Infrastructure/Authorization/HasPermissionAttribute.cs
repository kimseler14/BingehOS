using Microsoft.AspNetCore.Authorization;

namespace BingehOS.Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permissionName) : base("HasPermission")
    {
        Policy = $"HasPermission:{permissionName}";
    }
}

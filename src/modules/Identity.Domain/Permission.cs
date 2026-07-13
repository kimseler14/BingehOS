using BingehOS.Shared;

namespace BingehOS.Modules.Identity.Domain;

public class Permission : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Permission Create(Guid tenantId, string name, string? description)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            Description = description
        };
}

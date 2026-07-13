using BingehOS.Shared;

namespace BingehOS.Modules.Identity.Domain;

public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Role Create(Guid tenantId, string name, string? description, bool isSystem = false)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            Description = description,
            IsSystem = isSystem
        };
}

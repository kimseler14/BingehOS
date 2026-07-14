using BingehOS.Shared;

namespace BingehOS.Modules.Identity.Domain;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }

    public Role? Role { get; private set; }
    public Permission? Permission { get; private set; }

    public static RolePermission Create(Guid tenantId, Guid roleId, Guid permissionId)
        => new()
        {
            TenantId = tenantId,
            RoleId = roleId,
            PermissionId = permissionId,
            AssignedAt = DateTimeOffset.UtcNow
        };

    public void SoftDelete() => IsDeleted = true;
}

using BingehOS.Shared;

namespace BingehOS.Modules.Identity.Domain;

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }
    public Guid AssignedByUserId { get; private set; }

    public User? User { get; private set; }
    public Role? Role { get; private set; }

    public static UserRole Create(Guid tenantId, Guid userId, Guid roleId, Guid assignedByUserId)
        => new()
        {
            TenantId = tenantId,
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTimeOffset.UtcNow,
            AssignedByUserId = assignedByUserId
        };
}

using BingehOS.Shared;

namespace BingehOS.Modules.Audit.Domain;

public class AuditLog : BaseEntity
{
    public string EntityName { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public AuditAction Action { get; private set; }
    public string? ChangedBy { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }

    public static AuditLog Create(Guid tenantId, string entityName, Guid entityId, AuditAction action, string? changedBy, string? oldValues, string? newValues)
        => new()
        {
            TenantId = tenantId,
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            ChangedBy = changedBy,
            ChangedAt = DateTimeOffset.UtcNow,
            OldValues = oldValues,
            NewValues = newValues
        };
}
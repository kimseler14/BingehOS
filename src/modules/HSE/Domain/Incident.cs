using BingehOS.Shared;

namespace BingehOS.Modules.HSE.Domain;

public class Incident : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Severity { get; private set; } = "Low";
    public DateTime OccurredAt { get; private set; }
    public bool IsResolved { get; private set; }

    public static Incident Create(Guid tenantId, string title, string description, string severity, DateTime occurredAt, bool isResolved)
        => new() { TenantId = tenantId, Title = title, Description = description, Severity = severity, OccurredAt = occurredAt, IsResolved = isResolved };

    public void Rename(string title) => Title = title;
    public void ChangeDescription(string description) => Description = description;
    public void SetSeverity(string severity) => Severity = severity;
    public void Resolve() => IsResolved = true;
}

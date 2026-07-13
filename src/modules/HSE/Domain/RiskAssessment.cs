using BingehOS.Shared;

namespace BingehOS.Modules.HSE.Domain;

public class RiskAssessment : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Level { get; private set; } = "Low";
    public Guid PermitToWorkId { get; private set; }

    public static RiskAssessment Create(Guid tenantId, string title, string description, string level, Guid permitToWorkId)
        => new() { TenantId = tenantId, Title = title, Description = description, Level = level, PermitToWorkId = permitToWorkId };
}

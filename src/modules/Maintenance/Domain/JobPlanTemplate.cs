using BingehOS.Shared;

namespace BingehOS.Modules.Maintenance.Domain;

public class JobPlanTemplate : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string AssetType { get; private set; } = string.Empty;
    public string Steps { get; private set; } = string.Empty;
    public int? EstimatedDurationMinutes { get; private set; }
    public string RequiredPpe { get; private set; } = string.Empty;
    public string RequiredPermitType { get; private set; } = string.Empty;
    public string RecommendedParts { get; private set; } = string.Empty;

    public static JobPlanTemplate Create(Guid tenantId, string name, string description, string assetType, string steps, int? estimatedDurationMinutes, string requiredPpe, string requiredPermitType, string recommendedParts)
        => new() { TenantId = tenantId, Name = name, Description = description, AssetType = assetType, Steps = steps, EstimatedDurationMinutes = estimatedDurationMinutes, RequiredPpe = requiredPpe, RequiredPermitType = requiredPermitType, RecommendedParts = recommendedParts };

    public void Update(string name, string description, string assetType, string steps, int? estimatedDurationMinutes, string requiredPpe, string requiredPermitType, string recommendedParts)
    {
        Name = name;
        Description = description;
        AssetType = assetType;
        Steps = steps;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        RequiredPpe = requiredPpe;
        RequiredPermitType = requiredPermitType;
        RecommendedParts = recommendedParts;
    }
}

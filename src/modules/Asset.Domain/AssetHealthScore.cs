using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public class AssetHealthScore : BaseEntity
{
    public Guid AssetId { get; private set; }
    public int Score { get; private set; }
    public string? CalculationDetails { get; private set; }
    public DateTimeOffset CalculatedAt { get; private set; }

    public static AssetHealthScore Create(Guid tenantId, Guid assetId, int score, string? calculationDetails)
        => new()
        {
            TenantId = tenantId,
            AssetId = assetId,
            Score = score,
            CalculationDetails = calculationDetails,
            CalculatedAt = DateTimeOffset.UtcNow
        };

    public void UpdateScore(int score, string? calculationDetails)
    {
        Score = score;
        CalculationDetails = calculationDetails;
        CalculatedAt = DateTimeOffset.UtcNow;
    }
}

using BingehOS.Shared;

namespace BingehOS.Modules.Compliance.Domain;

public class CalibrationRecord : BaseEntity
{
    public Guid AssetId { get; private set; }
    public DateTimeOffset CalibratedAt { get; private set; }
    public DateTimeOffset? NextDueAt { get; private set; }
    public string Result { get; private set; } = string.Empty;

    public static CalibrationRecord Create(
        Guid tenantId,
        Guid assetId,
        DateTimeOffset calibratedAt,
        DateTimeOffset? nextDueAt,
        string result)
        => new()
        {
            TenantId = tenantId,
            AssetId = assetId,
            CalibratedAt = calibratedAt,
            NextDueAt = nextDueAt,
            Result = result
        };

    public void Update(
        Guid assetId,
        DateTimeOffset calibratedAt,
        DateTimeOffset? nextDueAt,
        string result)
    {
        AssetId = assetId;
        CalibratedAt = calibratedAt;
        NextDueAt = nextDueAt;
        Result = result;
    }
}

using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public class Meter : BaseEntity
{
    public Guid AssetId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public string? MeterType { get; private set; }
    public DateTimeOffset? LastReadingAt { get; private set; }
    public double? LastReadingValue { get; private set; }

    public static Meter Create(Guid tenantId, Guid assetId, string name, string unit, string? meterType)
        => new()
        {
            TenantId = tenantId,
            AssetId = assetId,
            Name = name,
            Unit = unit,
            MeterType = meterType
        };

    public void RecordReading(double value)
    {
        LastReadingAt = DateTimeOffset.UtcNow;
        LastReadingValue = value;
    }
}

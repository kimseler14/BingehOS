using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class DowntimeCost : BaseEntity
{
    public Guid AssetId { get; private set; }
    public Guid? WorkOrderId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset? EndTime { get; private set; }
    public long CostPerHourMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string? Notes { get; private set; }

    public static DowntimeCost Create(Guid tenantId, Guid assetId, Guid? workOrderId, DateTimeOffset startTime, long costPerHourMinor, string currency, string? notes)
        => new()
        {
            TenantId = tenantId,
            AssetId = assetId,
            WorkOrderId = workOrderId,
            StartTime = startTime,
            CostPerHourMinor = costPerHourMinor,
            Currency = currency,
            Notes = notes
        };

    public void Close(DateTimeOffset endTime)
    {
        EndTime = endTime;
    }
}

using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class CostAllocation : BaseEntity
{
    public Guid CostCenterId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public Guid? WorkOrderId { get; private set; }
    public Guid? AssetId { get; private set; }
    public long AllocatedAmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public DateTimeOffset AllocationDate { get; private set; }
    public string? Notes { get; private set; }

    public static CostAllocation Create(Guid tenantId, Guid costCenterId, long allocatedAmountMinor, string currency, DateTimeOffset allocationDate, Guid? invoiceId = null, Guid? workOrderId = null, Guid? assetId = null, string? notes = null)
        => new()
        {
            TenantId = tenantId,
            CostCenterId = costCenterId,
            InvoiceId = invoiceId,
            WorkOrderId = workOrderId,
            AssetId = assetId,
            AllocatedAmountMinor = allocatedAmountMinor,
            Currency = currency,
            AllocationDate = allocationDate,
            Notes = notes
        };
}

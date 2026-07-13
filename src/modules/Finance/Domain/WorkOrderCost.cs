using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class WorkOrderCost : BaseEntity
{
    public Guid WorkOrderId { get; private set; }
    public long AmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string Status { get; private set; } = "Pending";
    public bool IsApproved { get; private set; }

    public static WorkOrderCost Create(Guid tenantId, Guid workOrderId, long amountMinor, string currency, string status, bool isApproved)
        => new() { TenantId = tenantId, WorkOrderId = workOrderId, AmountMinor = amountMinor, Currency = currency, Status = status, IsApproved = isApproved };

    public void Approve() => IsApproved = true;
}

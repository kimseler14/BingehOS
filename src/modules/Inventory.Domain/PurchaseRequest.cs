using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class PurchaseRequest : BaseEntity
{
    public string RequestNumber { get; private set; } = string.Empty;
    public Guid RequestedByUserId { get; private set; }
    public PurchaseRequestStatus Status { get; private set; } = PurchaseRequestStatus.Draft;
    public string? Reason { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public string? Notes { get; private set; }

    public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = new List<PurchaseOrder>();

    public static PurchaseRequest Create(Guid tenantId, string requestNumber, Guid requestedByUserId, string? reason)
        => new()
        {
            TenantId = tenantId,
            RequestNumber = requestNumber,
            RequestedByUserId = requestedByUserId,
            Status = PurchaseRequestStatus.Draft,
            Reason = reason
        };

    public void Submit() => Status = PurchaseRequestStatus.Submitted;
    public void Approve(Guid approvedByUserId)
    {
        Status = PurchaseRequestStatus.Approved;
        ApprovedAt = DateTimeOffset.UtcNow;
        ApprovedByUserId = approvedByUserId;
    }
    public void Reject() => Status = PurchaseRequestStatus.Rejected;
}
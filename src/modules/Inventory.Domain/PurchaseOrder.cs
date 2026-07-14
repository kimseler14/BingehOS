using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid PurchaseRequestId { get; private set; }
    public Guid VendorId { get; private set; }
    public PurchaseOrderStatus Status { get; private set; } = PurchaseOrderStatus.Draft;
    public DateTimeOffset? ExpectedDeliveryDate { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? ReceivedAt { get; private set; }
    public string? Notes { get; private set; }

    public static PurchaseOrder Create(Guid tenantId, string orderNumber, Guid purchaseRequestId, Guid vendorId, DateTimeOffset? expectedDeliveryDate)
        => new()
        {
            TenantId = tenantId,
            OrderNumber = orderNumber,
            PurchaseRequestId = purchaseRequestId,
            VendorId = vendorId,
            Status = PurchaseOrderStatus.Draft,
            ExpectedDeliveryDate = expectedDeliveryDate
        };

    public void Send()
    {
        Status = PurchaseOrderStatus.Sent;
        SentAt = DateTimeOffset.UtcNow;
    }
    public void MarkReceived()
    {
        Status = PurchaseOrderStatus.Received;
        ReceivedAt = DateTimeOffset.UtcNow;
    }
    public void Cancel() => Status = PurchaseOrderStatus.Cancelled;
}
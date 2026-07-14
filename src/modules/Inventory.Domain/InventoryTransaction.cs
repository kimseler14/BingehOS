using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class InventoryTransaction : BaseEntity
{
    public Guid PartId { get; private set; }
    public Guid? BinId { get; private set; }
    public TransactionType Type { get; private set; }
    public int Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = "pcs";
    public Guid? RelatedWorkOrderId { get; private set; }
    public Guid? RelatedPurchaseOrderId { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }

    public static InventoryTransaction Create(Guid tenantId, Guid partId, Guid? binId, TransactionType type, int quantity, string unitOfMeasure, Guid? relatedWorkOrderId, Guid? relatedPurchaseOrderId, string? notes)
        => new()
        {
            TenantId = tenantId,
            PartId = partId,
            BinId = binId,
            Type = type,
            Quantity = quantity,
            UnitOfMeasure = unitOfMeasure,
            RelatedWorkOrderId = relatedWorkOrderId,
            RelatedPurchaseOrderId = relatedPurchaseOrderId,
            Notes = notes,
            TransactionDate = DateTimeOffset.UtcNow
        };
}
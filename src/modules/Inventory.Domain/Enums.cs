namespace BingehOS.Modules.Inventory.Domain;

public enum StockType
{
    SparePart,
    Consumable,
    Tool
}

public enum TransactionType
{
    Receiving,
    Issue,
    Return,
    Reservation,
    Transfer
}

public enum PurchaseRequestStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected,
    Ordered,
    Received
}

public enum PurchaseOrderStatus
{
    Draft,
    Sent,
    Acknowledged,
    Shipped,
    Received,
    Cancelled
}
using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public Guid? VendorId { get; private set; }
    public long TotalAmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string Status { get; private set; } = "Draft";
    public string Type { get; private set; } = "Purchase";

    public static Invoice Create(Guid tenantId, string invoiceNumber, DateTimeOffset invoiceDate, DateTimeOffset dueDate, Guid? vendorId, long totalAmountMinor, string currency, string status, string type)
        => new() { TenantId = tenantId, InvoiceNumber = invoiceNumber, InvoiceDate = invoiceDate, DueDate = dueDate, VendorId = vendorId, TotalAmountMinor = totalAmountMinor, Currency = currency, Status = status, Type = type };
}

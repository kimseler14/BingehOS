using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class TaxRecord : BaseEntity
{
    public Guid InvoiceId { get; private set; }
    public string TaxType { get; private set; } = "VAT";
    public decimal TaxRate { get; private set; }
    public long TaxAmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";

    public static TaxRecord Create(Guid tenantId, Guid invoiceId, string taxType, decimal taxRate, long taxAmountMinor, string currency)
        => new() { TenantId = tenantId, InvoiceId = invoiceId, TaxType = taxType, TaxRate = taxRate, TaxAmountMinor = taxAmountMinor, Currency = currency };
}

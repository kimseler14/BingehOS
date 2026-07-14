using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class EnergyCost : BaseEntity
{
    public Guid AssetId { get; private set; }
    public DateTimeOffset BillingPeriodStart { get; private set; }
    public DateTimeOffset BillingPeriodEnd { get; private set; }
    public long AmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string? EnergyType { get; private set; }
    public string? MeterNumber { get; private set; }
    public string? Provider { get; private set; }
    public string? DocumentUrl { get; private set; }

    public static EnergyCost Create(Guid tenantId, Guid assetId, DateTimeOffset billingPeriodStart, DateTimeOffset billingPeriodEnd, long amountMinor, string currency, string? energyType, string? meterNumber, string? provider, string? documentUrl)
        => new()
        {
            TenantId = tenantId,
            AssetId = assetId,
            BillingPeriodStart = billingPeriodStart,
            BillingPeriodEnd = billingPeriodEnd,
            AmountMinor = amountMinor,
            Currency = currency,
            EnergyType = energyType,
            MeterNumber = meterNumber,
            Provider = provider,
            DocumentUrl = documentUrl
        };
}

using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public class Warranty : BaseEntity
{
    public Guid AssetId { get; private set; }
    public string? Provider { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public string? DocumentUrl { get; private set; }
    public string? Notes { get; private set; }

    public static Warranty Create(Guid tenantId, Guid assetId, string? provider, DateTimeOffset startDate, DateTimeOffset endDate, string? documentUrl, string? notes)
        => new()
        {
            TenantId = tenantId,
            AssetId = assetId,
            Provider = provider,
            StartDate = startDate,
            EndDate = endDate,
            DocumentUrl = documentUrl,
            Notes = notes
        };

    public bool IsActive(DateTimeOffset referenceDate) => referenceDate >= StartDate && referenceDate <= EndDate;
}

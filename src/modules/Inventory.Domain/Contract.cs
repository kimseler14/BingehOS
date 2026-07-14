using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class Contract : BaseEntity
{
    public Guid VendorId { get; private set; }
    public string ContractNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public string? Terms { get; private set; }
    public string? DocumentUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static Contract Create(Guid tenantId, Guid vendorId, string contractNumber, string title, DateTimeOffset startDate, DateTimeOffset endDate, string? terms, string? documentUrl)
        => new()
        {
            TenantId = tenantId,
            VendorId = vendorId,
            ContractNumber = contractNumber,
            Title = title,
            StartDate = startDate,
            EndDate = endDate,
            Terms = terms,
            DocumentUrl = documentUrl,
            IsActive = true
        };

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
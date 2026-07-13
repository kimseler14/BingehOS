using FacilityOS.Shared;

namespace FacilityOS.Modules.Vendor.Domain;

public class Vendor : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? TaxNumber { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static Vendor Create(Guid tenantId, string name, string? taxNumber, string? contactEmail, string? phone, bool isActive)
        => new() { TenantId = tenantId, Name = name, TaxNumber = taxNumber, ContactEmail = contactEmail, Phone = phone, IsActive = isActive };

    public void Rename(string name) => Name = name;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

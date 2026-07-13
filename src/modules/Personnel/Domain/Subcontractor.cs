using BingehOS.Shared;

namespace BingehOS.Modules.Personnel.Domain;

public class Subcontractor : BaseEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string TaxNumber { get; private set; } = string.Empty;
    public string? ContactPerson { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static Subcontractor Create(Guid tenantId, string companyName, string taxNumber, string? contactPerson, string? phone, bool isActive)
        => new() { TenantId = tenantId, CompanyName = companyName, TaxNumber = taxNumber, ContactPerson = contactPerson, Phone = phone, IsActive = isActive };

    public void Update(string companyName, string taxNumber, string? contactPerson, string? phone)
    {
        CompanyName = companyName;
        TaxNumber = taxNumber;
        ContactPerson = contactPerson;
        Phone = phone;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

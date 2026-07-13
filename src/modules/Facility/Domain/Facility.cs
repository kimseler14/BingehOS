using FacilityOS.Shared;

namespace FacilityOS.Modules.Facility.Domain;

public class Facility : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Address { get; private set; }
    public string? TimeZone { get; private set; }
    public Guid? ParentFacilityId { get; private set; }

    public static Facility Create(Guid tenantId, string name, string? code, string? address, string? timeZone, Guid? parentFacilityId)
        => new() { TenantId = tenantId, Name = name, Code = code, Address = address, TimeZone = timeZone, ParentFacilityId = parentFacilityId };

    public void Rename(string name) => Name = name;
    public void ChangeAddress(string? address) => Address = address;
    public void SetTimeZone(string? timeZone) => TimeZone = timeZone;
}

using BingehOS.Shared;

namespace BingehOS.Modules.Facility.Domain;

public class Campus : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Address { get; private set; }
    public string? TimeZone { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    public ICollection<Building> Buildings { get; private set; } = new List<Building>();

    public static Campus Create(Guid tenantId, string name, string? code, string? address, string? timeZone, double? latitude, double? longitude)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            Address = address,
            TimeZone = timeZone,
            Latitude = latitude,
            Longitude = longitude
        };

    public void Rename(string name) => Name = name;
    public void SetLocation(double? latitude, double? longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}

using BingehOS.Shared;

namespace BingehOS.Modules.Facility.Domain;

public class Building : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Address { get; private set; }
    public Guid CampusId { get; private set; }
    public Campus? Campus { get; private set; }
    public string? TimeZone { get; private set; }
    public int? FloorsCount { get; private set; }

    public ICollection<Floor> Floors { get; private set; } = new List<Floor>();

    public static Building Create(Guid tenantId, Guid campusId, string name, string? code, string? address, string? timeZone, int? floorsCount)
        => new()
        {
            TenantId = tenantId,
            CampusId = campusId,
            Name = name,
            Code = code,
            Address = address,
            TimeZone = timeZone,
            FloorsCount = floorsCount
        };

    public void Rename(string name) => Name = name;
    public void SetFloorsCount(int count) => FloorsCount = count;
}

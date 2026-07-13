using BingehOS.Shared;

namespace BingehOS.Modules.Facility.Domain;

public class Floor : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public int Level { get; private set; }
    public Guid BuildingId { get; private set; }
    public Building? Building { get; private set; }

    public ICollection<Zone> Zones { get; private set; } = new List<Zone>();
    public ICollection<Room> Rooms { get; private set; } = new List<Room>();

    public static Floor Create(Guid tenantId, Guid buildingId, string name, string? code, int level)
        => new()
        {
            TenantId = tenantId,
            BuildingId = buildingId,
            Name = name,
            Code = code,
            Level = level
        };

    public void Rename(string name) => Name = name;
}

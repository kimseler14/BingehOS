using BingehOS.Shared;

namespace BingehOS.Modules.Facility.Domain;

public class Zone : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public Guid FloorId { get; private set; }
    public Floor? Floor { get; private set; }
    public SpatialType Type { get; private set; } = SpatialType.Zone;

    public ICollection<Room> Rooms { get; private set; } = new List<Room>();

    public static Zone Create(Guid tenantId, Guid floorId, string name, string? code, SpatialType type)
        => new()
        {
            TenantId = tenantId,
            FloorId = floorId,
            Name = name,
            Code = code,
            Type = type
        };

    public void Rename(string name) => Name = name;
}

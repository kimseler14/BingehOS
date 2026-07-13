using BingehOS.Shared;

namespace BingehOS.Modules.Facility.Domain;

public class Room : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public Guid? FloorId { get; private set; }
    public Floor? Floor { get; private set; }
    public Guid? ZoneId { get; private set; }
    public Zone? Zone { get; private set; }
    public SpatialType Type { get; private set; } = SpatialType.Room;
    public string? Description { get; private set; }

    public static Room Create(Guid tenantId, Guid? floorId, Guid? zoneId, string name, string? code, SpatialType type, string? description)
        => new()
        {
            TenantId = tenantId,
            FloorId = floorId,
            ZoneId = zoneId,
            Name = name,
            Code = code,
            Type = type,
            Description = description
        };

    public void Rename(string name) => Name = name;
}

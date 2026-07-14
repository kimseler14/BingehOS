using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class Shelf : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public Guid LocationId { get; private set; }
    public Location? Location { get; private set; }
    public string? Description { get; private set; }

    public ICollection<Bin> Bins { get; private set; } = new List<Bin>();

    public static Shelf Create(Guid tenantId, Guid locationId, string name, string? code, string? description)
        => new()
        {
            TenantId = tenantId,
            LocationId = locationId,
            Name = name,
            Code = code,
            Description = description
        };

    public void Rename(string name) => Name = name;
}
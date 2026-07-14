using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class Location : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Warehouse? Warehouse { get; private set; }
    public string? Description { get; private set; }

    public ICollection<Shelf> Shelves { get; private set; } = new List<Shelf>();

    public static Location Create(Guid tenantId, Guid warehouseId, string name, string? code, string? description)
        => new()
        {
            TenantId = tenantId,
            WarehouseId = warehouseId,
            Name = name,
            Code = code,
            Description = description
        };

    public void Rename(string name) => Name = name;
}
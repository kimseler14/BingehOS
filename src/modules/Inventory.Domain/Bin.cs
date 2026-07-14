using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class Bin : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public Guid ShelfId { get; private set; }
    public Shelf? Shelf { get; private set; }
    public double? MaxCapacity { get; private set; }
    public string? Description { get; private set; }

    public static Bin Create(Guid tenantId, Guid shelfId, string name, string? code, double? maxCapacity, string? description)
        => new()
        {
            TenantId = tenantId,
            ShelfId = shelfId,
            Name = name,
            Code = code,
            MaxCapacity = maxCapacity,
            Description = description
        };

    public void Rename(string name) => Name = name;
}
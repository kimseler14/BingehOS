using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public class Warehouse : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Address { get; private set; }
    public string? ManagerUserId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Location> Locations { get; private set; } = new List<Location>();

    public static Warehouse Create(Guid tenantId, string name, string? code, string? address, string? managerUserId)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            Address = address,
            ManagerUserId = managerUserId,
            IsActive = true
        };

    public void Rename(string name) => Name = name;
    public void Deactivate() => IsActive = false;
}
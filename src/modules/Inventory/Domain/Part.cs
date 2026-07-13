using FacilityOS.Shared;

namespace FacilityOS.Modules.Inventory.Domain;

public class Part : BaseEntity
{
    public string PartNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string UnitOfMeasure { get; private set; } = "pcs";
    public bool IsActive { get; private set; } = true;

    public static Part Create(Guid tenantId, string partNumber, string name, string? description, string unitOfMeasure, bool isActive)
        => new() { TenantId = tenantId, PartNumber = partNumber, Name = name, Description = description, UnitOfMeasure = unitOfMeasure, IsActive = isActive };

    public void Rename(string name) => Name = name;
    public void Deactivate() => IsActive = false;
}

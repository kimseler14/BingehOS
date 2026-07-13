using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public class AssetType : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid AssetClassId { get; private set; }
    public AssetClass? AssetClass { get; private set; }
    public string? Model { get; private set; }
    public string? Manufacturer { get; private set; }
    public int? ExpectedLifespanYears { get; private set; }

    public ICollection<AssetTemplate> AssetTemplates { get; private set; } = new List<AssetTemplate>();

    public static AssetType Create(Guid tenantId, Guid assetClassId, string name, string? description, string? model, string? manufacturer, int? expectedLifespanYears)
        => new()
        {
            TenantId = tenantId,
            AssetClassId = assetClassId,
            Name = name,
            Description = description,
            Model = model,
            Manufacturer = manufacturer,
            ExpectedLifespanYears = expectedLifespanYears
        };

    public void Rename(string name) => Name = name;
}

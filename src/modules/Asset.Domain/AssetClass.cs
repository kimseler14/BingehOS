using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public class AssetClass : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }

    public ICollection<AssetType> AssetTypes { get; private set; } = new List<AssetType>();

    public static AssetClass Create(Guid tenantId, string name, string? description, string? icon)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            Description = description,
            Icon = icon
        };

    public void Rename(string name) => Name = name;
}

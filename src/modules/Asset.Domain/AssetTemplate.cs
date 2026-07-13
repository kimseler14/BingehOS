using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public class AssetTemplate : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid AssetTypeId { get; private set; }
    public AssetType? AssetType { get; private set; }
    public string? Attributes { get; private set; }
    public bool IsPreseeded { get; private set; }

    public static AssetTemplate Create(Guid tenantId, Guid assetTypeId, string name, string? description, string? attributes, bool isPreseeded = false)
        => new()
        {
            TenantId = tenantId,
            AssetTypeId = assetTypeId,
            Name = name,
            Description = description,
            Attributes = attributes,
            IsPreseeded = isPreseeded
        };

    public void Rename(string name) => Name = name;
}

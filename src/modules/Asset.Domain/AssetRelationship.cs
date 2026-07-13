using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public class AssetRelationship : BaseEntity
{
    public Guid ParentAssetId { get; private set; }
    public Guid ChildAssetId { get; private set; }
    public string RelationshipType { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public static AssetRelationship Create(Guid tenantId, Guid parentAssetId, Guid childAssetId, string relationshipType, string? description)
        => new()
        {
            TenantId = tenantId,
            ParentAssetId = parentAssetId,
            ChildAssetId = childAssetId,
            RelationshipType = relationshipType,
            Description = description
        };
}

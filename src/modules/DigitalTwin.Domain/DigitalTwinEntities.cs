using BingehOS.Shared;

namespace BingehOS.Modules.DigitalTwin.Domain;

public sealed class FloorPlan : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid? FacilityId { get; private set; }
    public string? ImageUrl { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public static FloorPlan Create(
        Guid tenantId,
        string name,
        Guid? facilityId,
        string? imageUrl,
        int width,
        int height)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            FacilityId = facilityId,
            ImageUrl = imageUrl,
            Width = width,
            Height = height
        };

    public void Update(string name, Guid? facilityId, string? imageUrl, int width, int height)
    {
        Name = name;
        FacilityId = facilityId;
        ImageUrl = imageUrl;
        Width = width;
        Height = height;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

public sealed class AssetPosition : BaseEntity
{
    public Guid AssetId { get; private set; }
    public Guid FloorPlanId { get; private set; }
    public double X { get; private set; }
    public double Y { get; private set; }

    public static AssetPosition Create(Guid tenantId, Guid assetId, Guid floorPlanId, double x, double y)
        => new()
        {
            TenantId = tenantId,
            AssetId = assetId,
            FloorPlanId = floorPlanId,
            X = x,
            Y = y
        };

    public void Move(double x, double y)
    {
        X = x;
        Y = y;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

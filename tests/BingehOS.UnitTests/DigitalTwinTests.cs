using BingehOS.Modules.DigitalTwin.Domain;
using Xunit;

namespace BingehOS.UnitTests;

public class DigitalTwinTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void FloorPlan_UpdateAndSoftDelete_PreservesOverlayMetadata()
    {
        var plan = FloorPlan.Create(TenantId, "Zemin Kat", null, null, 1200, 800);
        plan.Update("Zemin Kat Revize", null, "https://example.test/plan.svg", 1600, 900);

        Assert.Equal("Zemin Kat Revize", plan.Name);
        Assert.Equal(1600, plan.Width);
        Assert.Equal(900, plan.Height);
        Assert.Equal("https://example.test/plan.svg", plan.ImageUrl);

        plan.SoftDelete();

        Assert.True(plan.IsDeleted);
    }

    [Fact]
    public void AssetPosition_Move_UpdatesRelativeCoordinates()
    {
        var position = AssetPosition.Create(TenantId, Guid.NewGuid(), Guid.NewGuid(), 0.2, 0.3);
        position.Move(0.75, 0.9);

        Assert.Equal(0.75, position.X);
        Assert.Equal(0.9, position.Y);
    }
}

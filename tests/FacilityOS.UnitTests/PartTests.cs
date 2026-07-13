using FacilityOS.Modules.Inventory.Domain;

namespace FacilityOS.UnitTests;

public class PartTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var part = Part.Create(tenant, "PN-100", "Bearing", "6001-2RS", "pcs", true);

        Assert.Equal(tenant, part.TenantId);
        Assert.Equal("PN-100", part.PartNumber);
        Assert.Equal("Bearing", part.Name);
        Assert.Equal("6001-2RS", part.Description);
        Assert.Equal("pcs", part.UnitOfMeasure);
        Assert.True(part.IsActive);
    }

    [Fact]
    public void Rename_And_Deactivate_Update_Fields()
    {
        var part = Part.Create(Guid.NewGuid(), "PN-100", "Bearing", null, "pcs", true);
        part.Rename("Bearing V2");
        part.Deactivate();

        Assert.Equal("Bearing V2", part.Name);
        Assert.False(part.IsActive);
    }
}

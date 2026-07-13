using FacilityOS.Modules.Asset.Domain;

namespace FacilityOS.UnitTests;

public class AssetTests
{
    [Fact]
    public void Create_Sets_Tenant_And_Fields()
    {
        var tenant = Guid.NewGuid();
        var asset = Asset.Create(tenant, "Pump A", "SN-1", "B1", AssetCriticality.High);

        Assert.Equal(tenant, asset.TenantId);
        Assert.Equal("Pump A", asset.Name);
        Assert.Equal("SN-1", asset.SerialNumber);
        Assert.Equal("B1", asset.LocationCode);
        Assert.Equal(AssetCriticality.High, asset.Criticality);
    }

    [Fact]
    public void Rename_ChangeLocation_SetCriticality_Update_Fields()
    {
        var asset = Asset.Create(Guid.NewGuid(), "Old", null, "B1", AssetCriticality.Normal);

        asset.Rename("New");
        asset.ChangeLocation("B2");
        asset.SetCriticality(AssetCriticality.Critical);

        Assert.Equal("New", asset.Name);
        Assert.Equal("B2", asset.LocationCode);
        Assert.Equal(AssetCriticality.Critical, asset.Criticality);
    }
}

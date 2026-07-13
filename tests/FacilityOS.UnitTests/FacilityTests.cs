using FacilityOS.Modules.Facility.Domain;

namespace FacilityOS.UnitTests;

public class FacilityTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var parent = Guid.NewGuid();
        var facility = Facility.Create(tenant, "HQ", "HQ-1", "Istanbul", "Europe/Istanbul", parent);

        Assert.Equal(tenant, facility.TenantId);
        Assert.Equal("HQ", facility.Name);
        Assert.Equal("HQ-1", facility.Code);
        Assert.Equal("Istanbul", facility.Address);
        Assert.Equal("Europe/Istanbul", facility.TimeZone);
        Assert.Equal(parent, facility.ParentFacilityId);
    }

    [Fact]
    public void Rename_ChangeAddress_SetTimeZone_Updates_Fields()
    {
        var facility = Facility.Create(Guid.NewGuid(), "Old", null, null, null, null);

        facility.Rename("New");
        facility.ChangeAddress("Ankara");
        facility.SetTimeZone("Europe/Istanbul");

        Assert.Equal("New", facility.Name);
        Assert.Equal("Ankara", facility.Address);
        Assert.Equal("Europe/Istanbul", facility.TimeZone);
    }
}

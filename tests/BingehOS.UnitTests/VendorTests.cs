using BingehOS.Modules.Vendor.Domain;

namespace BingehOS.UnitTests;

public class VendorTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var vendor = Vendor.Create(tenant, "Acme Ltd", "1234567890", "info@acme.test", "+90 555 000 00 00", true);

        Assert.Equal(tenant, vendor.TenantId);
        Assert.Equal("Acme Ltd", vendor.Name);
        Assert.Equal("1234567890", vendor.TaxNumber);
        Assert.Equal("info@acme.test", vendor.ContactEmail);
        Assert.Equal("+90 555 000 00 00", vendor.Phone);
        Assert.True(vendor.IsActive);
    }

    [Fact]
    public void Rename_Activate_Deactivate_Work()
    {
        var vendor = Vendor.Create(Guid.NewGuid(), "Old", null, null, null, true);
        vendor.Rename("New");
        vendor.Deactivate();
        Assert.Equal("New", vendor.Name);
        Assert.False(vendor.IsActive);
        vendor.Activate();
        Assert.True(vendor.IsActive);
    }
}

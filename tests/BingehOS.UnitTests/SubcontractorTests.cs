using BingehOS.Modules.Personnel.Domain;

namespace BingehOS.UnitTests;

public class SubcontractorTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var subcontractor = Subcontractor.Create(Guid.NewGuid(), "Acme Ltd", "TAX-123", "John Doe", "+90 555 000 00 00", true);
        Assert.Equal("Acme Ltd", subcontractor.CompanyName);
        Assert.Equal("TAX-123", subcontractor.TaxNumber);
        Assert.Equal("John Doe", subcontractor.ContactPerson);
        Assert.True(subcontractor.IsActive);
    }

    [Fact]
    public void Update_Changes_Fields()
    {
        var subcontractor = Subcontractor.Create(Guid.NewGuid(), "Acme Ltd", "TAX-123", null, null, true);
        subcontractor.Update("Beta Inc", "TAX-456", "Jane Smith", "+90 555 111 11 11");
        Assert.Equal("Beta Inc", subcontractor.CompanyName);
        Assert.Equal("TAX-456", subcontractor.TaxNumber);
        Assert.Equal("Jane Smith", subcontractor.ContactPerson);
        Assert.Equal("+90 555 111 11 11", subcontractor.Phone);
    }

    [Fact]
    public void Activate_And_Deactivate_Work()
    {
        var subcontractor = Subcontractor.Create(Guid.NewGuid(), "Acme Ltd", "TAX-123", null, null, true);
        subcontractor.Deactivate();
        Assert.False(subcontractor.IsActive);
        subcontractor.Activate();
        Assert.True(subcontractor.IsActive);
    }
}

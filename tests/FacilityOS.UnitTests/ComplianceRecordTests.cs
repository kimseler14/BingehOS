using FacilityOS.Modules.Compliance.Domain;

namespace FacilityOS.UnitTests;

public class ComplianceRecordTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var record = ComplianceRecord.Create(tenant, "ISO Audit", "Annual ISO 55001 audit", "Pending", DateTime.UtcNow.AddDays(30));
        Assert.Equal(tenant, record.TenantId);
        Assert.Equal("ISO Audit", record.Title);
        Assert.Equal("Pending", record.Status);
    }

    [Fact]
    public void Update_Changes_Fields()
    {
        var record = ComplianceRecord.Create(Guid.NewGuid(), "Old", "Old desc", "Pending", DateTime.UtcNow);
        record.Update("New", "New desc", "Completed", DateTime.UtcNow);
        Assert.Equal("New", record.Title);
        Assert.Equal("Completed", record.Status);
    }
}

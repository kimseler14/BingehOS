using BingehOS.Modules.HSE.Domain;

namespace BingehOS.UnitTests;

public class IncidentTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var incident = Incident.Create(tenant, "Slip", "Wet floor", "Medium", DateTime.UtcNow, false);
        Assert.Equal(tenant, incident.TenantId);
        Assert.Equal("Slip", incident.Title);
        Assert.Equal("Medium", incident.Severity);
        Assert.False(incident.IsResolved);
    }

    [Fact]
    public void Resolve_Sets_IsResolved()
    {
        var incident = Incident.Create(Guid.NewGuid(), "Fire", "Small fire", "High", DateTime.UtcNow, false);
        incident.Resolve();
        Assert.True(incident.IsResolved);
    }
}

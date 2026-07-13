using BingehOS.Modules.HSE.Domain;

namespace BingehOS.UnitTests;

public class RiskAssessmentTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var permitId = Guid.NewGuid();
        var assessment = RiskAssessment.Create(tenant, "Fire Risk", "Assess fire hazard", "High", permitId);
        Assert.Equal(tenant, assessment.TenantId);
        Assert.Equal("Fire Risk", assessment.Title);
        Assert.Equal("High", assessment.Level);
        Assert.Equal(permitId, assessment.PermitToWorkId);
    }
}

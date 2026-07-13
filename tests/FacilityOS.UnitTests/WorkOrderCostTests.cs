using FacilityOS.Modules.Finance.Domain;

namespace FacilityOS.UnitTests;

public class WorkOrderCostTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var wo = Guid.NewGuid();
        var cost = WorkOrderCost.Create(tenant, wo, 150000, "TRY", "Pending", false);
        Assert.Equal(wo, cost.WorkOrderId);
        Assert.Equal(150000, cost.AmountMinor);
        Assert.Equal("TRY", cost.Currency);
        Assert.False(cost.IsApproved);
    }

    [Fact]
    public void Approve_Sets_IsApproved()
    {
        var cost = WorkOrderCost.Create(Guid.NewGuid(), Guid.NewGuid(), 150000, "TRY", "Pending", false);
        cost.Approve();
        Assert.True(cost.IsApproved);
    }
}

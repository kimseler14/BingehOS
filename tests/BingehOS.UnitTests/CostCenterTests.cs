using BingehOS.Modules.Finance.Domain;

namespace BingehOS.UnitTests;

public class CostCenterTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var parent = Guid.NewGuid();
        var costCenter = CostCenter.Create(tenant, "CC-001", "Operations", parent, 500000, "TRY", true);
        Assert.Equal("CC-001", costCenter.Code);
        Assert.Equal("Operations", costCenter.Name);
        Assert.Equal(parent, costCenter.ParentCostCenterId);
        Assert.Equal(500000, costCenter.BudgetMinor);
        Assert.Equal("TRY", costCenter.Currency);
        Assert.True(costCenter.IsActive);
    }
}

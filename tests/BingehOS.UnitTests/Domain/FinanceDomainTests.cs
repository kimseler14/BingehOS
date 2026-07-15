using BingehOS.Modules.Finance.Domain;

namespace BingehOS.UnitTests.Domain;

public class FinanceDomainTests
{
    [Fact]
    public void Budget_Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var costCenterId = Guid.NewGuid();
        var budget = Budget.Create(tenant, costCenterId, BudgetType.CapEx, 2025, 5_000_00, "TRY", "annual");

        Assert.Equal(tenant, budget.TenantId);
        Assert.Equal(costCenterId, budget.CostCenterId);
        Assert.Equal(BudgetType.CapEx, budget.Type);
        Assert.Equal(2025, budget.Year);
        Assert.Equal(5_000_00, budget.PlannedAmountMinor);
        Assert.Equal("TRY", budget.Currency);
        Assert.Equal("annual", budget.Notes);
    }

    [Fact]
    public void CostAllocation_Create_With_Optional_Links()
    {
        var tenant = Guid.NewGuid();
        var costCenterId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var woId = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var date = DateTimeOffset.UtcNow;

        var alloc = CostAllocation.Create(tenant, costCenterId, 1_250_00, "USD", date, invoiceId, woId, assetId, "note");

        Assert.Equal(costCenterId, alloc.CostCenterId);
        Assert.Equal(invoiceId, alloc.InvoiceId);
        Assert.Equal(woId, alloc.WorkOrderId);
        Assert.Equal(assetId, alloc.AssetId);
        Assert.Equal(1_250_00, alloc.AllocatedAmountMinor);
        Assert.Equal("USD", alloc.Currency);
        Assert.Equal(date, alloc.AllocationDate);
        Assert.Equal("note", alloc.Notes);
    }

    [Fact]
    public void CostAllocation_Create_Defaults_Optional_To_Null()
    {
        var alloc = CostAllocation.Create(Guid.NewGuid(), Guid.NewGuid(), 100, "TRY", DateTimeOffset.UtcNow);
        Assert.Null(alloc.InvoiceId);
        Assert.Null(alloc.WorkOrderId);
        Assert.Null(alloc.AssetId);
        Assert.Null(alloc.Notes);
    }

    [Fact]
    public void EnergyCost_Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var start = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero);
        var ec = EnergyCost.Create(tenant, assetId, start, end, 900_00, "TRY", "Electricity", "M-42", "GridCo", "http://bill");

        Assert.Equal(assetId, ec.AssetId);
        Assert.Equal(start, ec.BillingPeriodStart);
        Assert.Equal(end, ec.BillingPeriodEnd);
        Assert.Equal(900_00, ec.AmountMinor);
        Assert.Equal("TRY", ec.Currency);
        Assert.Equal("Electricity", ec.EnergyType);
        Assert.Equal("M-42", ec.MeterNumber);
        Assert.Equal("GridCo", ec.Provider);
        Assert.Equal("http://bill", ec.DocumentUrl);
    }

    [Fact]
    public void DowntimeCost_Create_And_Close()
    {
        var tenant = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var woId = Guid.NewGuid();
        var start = DateTimeOffset.UtcNow;
        var dc = DowntimeCost.Create(tenant, assetId, woId, start, 500_00, "TRY", "line stop");

        Assert.Equal(assetId, dc.AssetId);
        Assert.Equal(woId, dc.WorkOrderId);
        Assert.Equal(start, dc.StartTime);
        Assert.Equal(500_00, dc.CostPerHourMinor);
        Assert.Equal("TRY", dc.Currency);
        Assert.Equal("line stop", dc.Notes);
        Assert.Null(dc.EndTime);

        var end = start.AddHours(3);
        dc.Close(end);
        Assert.Equal(end, dc.EndTime);
    }
}

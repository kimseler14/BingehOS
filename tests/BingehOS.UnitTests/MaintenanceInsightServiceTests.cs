using BingehOS.Modules.Inventory.Domain;
using BingehOS.Modules.Maintenance.Application;
using Xunit;

namespace BingehOS.UnitTests;

public class MaintenanceInsightServiceTests
{
    [Fact]
    public void AssetInsight_FlagsElevatedRecentFailureRate()
    {
        var now = new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dates = new[]
        {
            now.AddDays(-250),
            now.AddDays(-230),
            now.AddDays(-20),
            now.AddDays(-10)
        };

        var insight = MaintenanceInsightService.CalculateAssetInsight(
            Guid.NewGuid(),
            "Kompresör",
            dates,
            now);

        Assert.True(insight.ElevatedRecentFailureRate);
        Assert.Equal("Elevated", insight.Trend);
        Assert.Equal(4, insight.FailureCount);
        Assert.NotNull(insight.MeanTimeBetweenFailuresDays);
    }

    [Fact]
    public void AssetInsight_ComputesMeanTimeBetweenFailures()
    {
        var now = new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dates = new[]
        {
            now.AddDays(-60),
            now.AddDays(-30),
            now
        };

        var insight = MaintenanceInsightService.CalculateAssetInsight(
            Guid.NewGuid(),
            "Pompa",
            dates,
            now);

        Assert.Equal(30, insight.MeanTimeBetweenFailuresDays);
    }

    [Fact]
    public void PartInsight_SuggestsReorderWhenStockIsBelowDerivedThreshold()
    {
        var now = new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var history = new List<(TransactionType Type, int Quantity, DateTimeOffset Date)>
        {
            (TransactionType.Receiving, 10, now.AddDays(-170)),
            (TransactionType.Issue, 4, now.AddDays(-120)),
            (TransactionType.Issue, 4, now.AddDays(-60)),
            (TransactionType.Issue, 1, now.AddDays(-10))
        };

        var insight = MaintenanceInsightService.CalculatePartInsight(
            Guid.NewGuid(),
            "FLT-01",
            "Filtre",
            history,
            now);

        Assert.NotNull(insight);
        Assert.Equal(1, insight!.CurrentStock);
        Assert.Equal(3, insight.SuggestedReorderThreshold);
        Assert.True(insight.AverageMonthlyIssueQuantity > 0);
    }

    [Fact]
    public void CorrectiveClassification_RecognizesTurkishFailureTerms()
    {
        Assert.True(MaintenanceInsightService.IsCorrective("Acil arıza onarımı"));
        Assert.True(MaintenanceInsightService.IsCorrective("Corrective maintenance"));
        Assert.False(MaintenanceInsightService.IsCorrective("Planlı aylık bakım"));
    }
}

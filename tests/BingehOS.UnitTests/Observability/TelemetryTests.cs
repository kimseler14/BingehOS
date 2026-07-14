using BingehOS.Shared.Telemetry;
using System.Diagnostics;
using Xunit;

namespace BingehOS.UnitTests;

public class TelemetryTests
{
    [Fact]
    public void ActivitySource_Name_IsBingehOS()
    {
        Assert.Equal("BingehOS", BingehOSActivitySource.SourceName);
        Assert.NotNull(BingehOSActivitySource.Source);
        Assert.Equal("BingehOS", BingehOSActivitySource.Source.Name);
    }

    [Fact]
    public void StartActivity_CreatesSampledActivity_WhenListenerRegistered()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == BingehOSActivitySource.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = _ => { },
            ActivityStopped = _ => { }
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = BingehOSActivitySource.Source.StartActivity("WorkOrder.Create");
        Assert.NotNull(activity);
        Assert.Equal("WorkOrder.Create", activity!.OperationName);
        Assert.Equal("BingehOS", activity.Source.Name);
        activity.SetTag("tenant.id", "test-tenant");
        Assert.Equal("test-tenant", activity.GetTagItem("tenant.id")?.ToString());
    }
}

namespace BingehOS.Shared.Telemetry;

using System.Diagnostics;

/// <summary>
/// Shared OpenTelemetry <see cref="ActivitySource"/> for BingehOS.
/// Modules create business-level spans from this single source so that the
/// OTel tracer (configured with <c>AddSource(BingehOSActivitySource.SourceName)</c>)
/// picks them up and exports them for distributed tracing.
/// </summary>
public static class BingehOSActivitySource
{
    /// <summary>The well-known name used to register this source with the tracer.</summary>
    public const string SourceName = "BingehOS";

    public static readonly ActivitySource Source = new(
        SourceName,
        typeof(BingehOSActivitySource).Assembly.GetName().Version?.ToString() ?? "1.0.0");
}

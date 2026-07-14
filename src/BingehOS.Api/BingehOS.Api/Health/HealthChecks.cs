using Microsoft.Extensions.Diagnostics.HealthChecks;
using BingehOS.Infrastructure;

namespace BingehOS.Api.Health;

public class BingehOSHealthCheck : IHealthCheck
{
    private readonly AppDbContext _db;

    public BingehOSHealthCheck(AppDbContext db) => _db = db;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            await _db.Database.CanConnectAsync(ct);
            return HealthCheckResult.Healthy("Database connection successful");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}

/// <summary>Liveness probe: the process is up. Always healthy.</summary>
public class SelfHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default) =>
        Task.FromResult(HealthCheckResult.Healthy("API is running"));
}

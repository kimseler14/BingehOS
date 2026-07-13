using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FacilityOS.Api.Health;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IServiceProvider _services;
    public HealthController(IServiceProvider services) => _services = services;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var checks = new List<(string Name, HealthStatus Status, string? Description)>
        {
            ("self", HealthStatus.Healthy, "API is running")
        };

        try
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetService<FacilityOS.Infrastructure.AppDbContext>();
            if (db != null)
            {
                checks.Add(("postgres", HealthStatus.Healthy, "DbContext registered"));
            }
        }
        catch (Exception ex)
        {
            checks.Add(("postgres", HealthStatus.Unhealthy, ex.Message));
        }

        var overall = checks.All(c => c.Status == HealthStatus.Healthy)
            ? HealthStatus.Healthy
            : HealthStatus.Degraded;

        return overall == HealthStatus.Healthy
            ? Ok(new { status = "healthy", checks })
            : StatusCode(503, new { status = "unhealthy", checks });
    }
}

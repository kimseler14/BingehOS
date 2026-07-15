using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Maintenance.Application;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/insights")]
[Authorize]
public sealed class InsightsController(MaintenanceInsightService service) : ControllerBase
{
    [HttpGet("assets")]
    [HasPermission("insights.read")]
    public async Task<IActionResult> Assets(CancellationToken cancellationToken)
        => this.OkWithData(await service.GetAssetInsightsAsync(cancellationToken));

    [HttpGet("parts")]
    [HasPermission("insights.read")]
    public async Task<IActionResult> Parts(CancellationToken cancellationToken)
        => this.OkWithData(await service.GetPartInsightsAsync(cancellationToken));
}

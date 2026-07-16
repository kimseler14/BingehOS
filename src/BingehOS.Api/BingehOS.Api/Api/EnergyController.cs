using BingehOS.Modules.Asset.Application;
using BingehOS.Modules.Finance.Application;
using BingehOS.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/energy")]
[Authorize]
public class EnergyController : ControllerBase
{
    private readonly IMediator _mediator;
    public EnergyController(IMediator mediator) => _mediator = mediator;

    [HttpGet("meters")]
    public async Task<IActionResult> GetMeters([FromQuery] Guid assetId)
    {
        var items = await _mediator.Send(new GetMetersQuery(assetId));
        return this.OkWithData(items);
    }

    [HttpPost("meters/{id:guid}/reading")]
    public async Task<IActionResult> RecordReading(Guid id, [FromBody] RecordMeterReadingBody body)
    {
        await _mediator.Send(new RecordMeterReadingCommand(id, body.Value));
        return this.NoContent();
    }

    [HttpGet("costs")]
    public async Task<IActionResult> GetCosts([FromQuery] Guid? assetId, [FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var items = await _mediator.Send(new GetEnergyCostsQuery(assetId, skip, take));
        return this.OkWithData(items);
    }
}

public record RecordMeterReadingBody(double Value);

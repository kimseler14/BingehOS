using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.DigitalTwin.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/floor-plans")]
[Authorize]
public sealed class FloorPlansController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [HasPermission("floor-plans.read")]
    public async Task<IActionResult> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
        => this.OkWithData(await mediator.Send(new GetFloorPlansQuery(skip, take)));

    [HttpGet("{id:guid}")]
    [HasPermission("floor-plans.read")]
    public async Task<IActionResult> Get(Guid id)
        => this.OkOrNotFound(await mediator.Send(new GetFloorPlanQuery(id)));

    [HttpPost]
    [HasPermission("floor-plans.write")]
    public async Task<IActionResult> Create([FromBody] CreateFloorPlanCommand command)
    {
        var id = await mediator.Send(command);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id:guid}")]
    [HasPermission("floor-plans.write")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFloorPlanCommand command)
    {
        if (command.Id != id)
            return this.IdMismatch();

        return this.OkOrNotFound(await mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("floor-plans.write")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await mediator.Send(new DeleteFloorPlanCommand(id));
        return deleted ? this.OkWithData(new { deleted = true }) : NotFound();
    }

    [HttpGet("{id:guid}/positions")]
    [HasPermission("floor-plans.read")]
    public async Task<IActionResult> Positions(Guid id)
        => this.OkWithData(await mediator.Send(new GetAssetPositionsQuery(id)));

    [HttpPut("{id:guid}/positions")]
    [HasPermission("floor-plans.write")]
    public async Task<IActionResult> ReplacePositions(
        Guid id,
        [FromBody] ReplacePositionsRequest request)
        => this.OkWithData(await mediator.Send(new ReplaceAssetPositionsCommand(id, request.Positions)));
}

public sealed record ReplacePositionsRequest(IReadOnlyList<AssetPositionInput> Positions);

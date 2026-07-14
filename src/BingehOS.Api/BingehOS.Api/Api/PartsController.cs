using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Inventory.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/parts")]
[Authorize]
public class PartsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PartsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartCommand cmd)
    {
        if (cmd.Id != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetPartQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] bool? activeOnly = null)
    {
        var items = await _mediator.Send(new GetPartsQuery(skip, take, activeOnly));
        return this.OkWithData(items);
    }

    [HttpPost("{id}/receive")]
    [HasPermission("inventory-transactions.write")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceivePartCommand cmd)
    {
        if (cmd.PartId != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpPost("{id}/issue")]
    [HasPermission("inventory-transactions.write")]
    public async Task<IActionResult> Issue(Guid id, [FromBody] IssuePartCommand cmd)
    {
        if (cmd.PartId != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpPost("{id}/return")]
    [HasPermission("inventory-transactions.write")]
    public async Task<IActionResult> Return(Guid id, [FromBody] ReturnPartCommand cmd)
    {
        if (cmd.PartId != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }
}

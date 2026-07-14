using BingehOS.Modules.Maintenance.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/work-orders")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public WorkOrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id }, new { success = true, data = new { id } });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetWorkOrderQuery(id));
        if (item == null) return NotFound(new { success = false, error = "not found" });
        return Ok(new { success = true, data = item });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var items = await _mediator.Send(new GetWorkOrdersQuery(skip, take));
        return Ok(new { success = true, data = items });
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeWorkOrderStatusCommand cmd)
    {
        if (cmd.Id != id) return BadRequest(new { error = "id mismatch" });
        var dto = await _mediator.Send(cmd);
        return Ok(new { success = true, data = dto });
    }
}

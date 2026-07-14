using BingehOS.Modules.HSE.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/permits")]
[Authorize]
public class PermitsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PermitsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermitToWorkCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id }, new { success = true, data = new { id } });
    }

    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovePermitToWorkCommand cmd)
    {
        if (cmd.Id != id) return BadRequest(new { error = "id mismatch" });
        var dto = await _mediator.Send(cmd);
        return Ok(new { success = true, data = dto });
    }

    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectPermitToWorkCommand cmd)
    {
        if (cmd.Id != id) return BadRequest(new { error = "id mismatch" });
        var dto = await _mediator.Send(cmd);
        return Ok(new { success = true, data = dto });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetPermitToWorkQuery(id));
        if (item == null) return NotFound(new { success = false, error = "not found" });
        return Ok(new { success = true, data = item });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] string? status = null)
    {
        var items = await _mediator.Send(new GetPermitsToWorkQuery(skip, take, status));
        return Ok(new { success = true, data = items });
    }
}

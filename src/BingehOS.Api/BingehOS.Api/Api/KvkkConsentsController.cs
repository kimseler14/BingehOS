using BingehOS.Modules.Compliance.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/kvkk-consents")]
[Authorize]
public class KvkkConsentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public KvkkConsentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKvkkConsentCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id }, new { success = true, data = new { id } });
    }

    [HttpPatch("{id}/revoke")]
    public async Task<IActionResult> Revoke(Guid id, [FromBody] RevokeKvkkConsentCommand cmd)
    {
        if (cmd.Id != id) return BadRequest(new { error = "id mismatch" });
        var dto = await _mediator.Send(cmd);
        return Ok(new { success = true, data = dto });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetKvkkConsentQuery(id));
        if (item == null) return NotFound(new { success = false, error = "not found" });
        return Ok(new { success = true, data = item });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] Guid? userId = null)
    {
        var items = await _mediator.Send(new GetKvkkConsentsQuery(skip, take, userId));
        return Ok(new { success = true, data = items });
    }
}

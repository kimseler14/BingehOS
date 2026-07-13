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
    public IActionResult Get(Guid id) => Ok(new { success = true, data = new { id } });
}

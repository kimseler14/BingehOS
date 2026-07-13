using BingehOS.Modules.HSE.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/loto-procedures")]
[Authorize]
public class LotoProceduresController : ControllerBase
{
    private readonly IMediator _mediator;
    public LotoProceduresController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLotoProcedureCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id }, new { success = true, data = new { id } });
    }

    [HttpPatch("{id}/verify")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] VerifyLotoProcedureCommand cmd)
    {
        if (cmd.Id != id) return BadRequest(new { error = "id mismatch" });
        var dto = await _mediator.Send(cmd);
        return Ok(new { success = true, data = dto });
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id) => Ok(new { success = true, data = new { id } });
}

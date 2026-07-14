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
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetLotoProcedureQuery(id));
        if (item == null) return NotFound(new { success = false, error = "not found" });
        return Ok(new { success = true, data = item });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] Guid? permitToWorkId = null)
    {
        var items = await _mediator.Send(new GetLotoProceduresQuery(skip, take, permitToWorkId));
        return Ok(new { success = true, data = items });
    }
}

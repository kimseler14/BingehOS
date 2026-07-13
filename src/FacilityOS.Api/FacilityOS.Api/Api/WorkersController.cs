using FacilityOS.Modules.Personnel.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FacilityOS.Api.Api;

[ApiController]
[Route("v1/workers")]
public class WorkersController : ControllerBase
{
    private readonly IMediator _mediator;
    public WorkersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkerCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkerCommand cmd)
    {
        if (cmd.Id != id) return BadRequest(new { error = "id mismatch" });
        var dto = await _mediator.Send(cmd);
        return Ok(new { success = true, data = dto });
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id) => Ok(new { id });
}

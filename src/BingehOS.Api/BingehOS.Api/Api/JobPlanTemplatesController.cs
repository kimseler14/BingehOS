using BingehOS.Modules.Maintenance.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/job-plan-templates")]
[Authorize]
public class JobPlanTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;
    public JobPlanTemplatesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobPlanTemplateCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id }, new { success = true, data = new { id } });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobPlanTemplateCommand cmd)
    {
        if (cmd.Id != id) return BadRequest(new { error = "id mismatch" });
        var dto = await _mediator.Send(cmd);
        return Ok(new { success = true, data = dto });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetJobPlanTemplateQuery(id));
        if (item == null) return NotFound(new { success = false, error = "not found" });
        return Ok(new { success = true, data = item });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] string? assetType = null)
    {
        var items = await _mediator.Send(new GetJobPlanTemplatesQuery(skip, take, assetType));
        return Ok(new { success = true, data = items });
    }
}

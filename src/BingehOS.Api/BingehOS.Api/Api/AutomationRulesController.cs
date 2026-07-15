using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Automation.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/automation-rules")]
[Authorize]
public sealed class AutomationRulesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [HasPermission("automation-rules.read")]
    public async Task<IActionResult> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
        => this.OkWithData(await mediator.Send(new GetAutomationRulesQuery(skip, take)));

    [HttpGet("{id:guid}")]
    [HasPermission("automation-rules.read")]
    public async Task<IActionResult> Get(Guid id)
        => this.OkOrNotFound(await mediator.Send(new GetAutomationRuleQuery(id)));

    [HttpPost]
    [HasPermission("automation-rules.write")]
    public async Task<IActionResult> Create([FromBody] CreateAutomationRuleCommand command)
    {
        var id = await mediator.Send(command);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id:guid}")]
    [HasPermission("automation-rules.write")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAutomationRuleCommand command)
    {
        if (command.Id != id)
            return this.IdMismatch();

        return this.OkOrNotFound(await mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("automation-rules.write")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await mediator.Send(new DeleteAutomationRuleCommand(id));
        return deleted ? this.OkWithData(new { deleted = true }) : NotFound();
    }

    [HttpGet("{id:guid}/executions")]
    [HasPermission("automation-rules.read")]
    public async Task<IActionResult> Executions(
        Guid id,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
        => this.OkWithData(await mediator.Send(new GetAutomationRuleExecutionsQuery(id, skip, take)));
}

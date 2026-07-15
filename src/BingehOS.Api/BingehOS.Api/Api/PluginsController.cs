using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Plugin.Application;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/plugins")]
[Authorize]
public sealed class PluginsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [HasPermission("plugins.read")]
    public async Task<IActionResult> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
        => this.OkWithData(await mediator.Send(new GetPluginRegistrationsQuery(skip, take)));

    [HttpGet("{id:guid}")]
    [HasPermission("plugins.read")]
    public async Task<IActionResult> Get(Guid id)
        => this.OkOrNotFound(await mediator.Send(new GetPluginRegistrationQuery(id)));

    [HttpPost]
    [HasPermission("plugins.write")]
    public async Task<IActionResult> Register([FromBody] CreatePluginRegistrationCommand command)
    {
        var id = await mediator.Send(command);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id:guid}")]
    [HasPermission("plugins.write")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePluginRegistrationCommand command)
    {
        if (command.Id != id)
            return this.IdMismatch();

        return this.OkOrNotFound(await mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("plugins.write")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await mediator.Send(new DeletePluginRegistrationCommand(id));
        return deleted ? this.OkWithData(new { deleted = true }) : NotFound();
    }
}

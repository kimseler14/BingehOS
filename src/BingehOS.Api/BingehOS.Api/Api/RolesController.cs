using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Identity.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RolesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [HasPermission("admin.access")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var items = await _mediator.Send(new GetRolesQuery(skip, take));
        return this.OkWithData(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetRoleQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpPatch("{id}")]
    [HasPermission("admin.access")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand cmd)
    {
        if (cmd.Id != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpDelete("{id}")]
    [HasPermission("admin.access")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteRoleCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/permissions/{permissionId}")]
    [HasPermission("admin.access")]
    public async Task<IActionResult> AddPermission(Guid id, Guid permissionId)
    {
        await _mediator.Send(new AddPermissionToRoleCommand(id, permissionId));
        return NoContent();
    }

    [HttpDelete("{id}/permissions/{permissionId}")]
    [HasPermission("admin.access")]
    public async Task<IActionResult> RemovePermission(Guid id, Guid permissionId)
    {
        await _mediator.Send(new RemovePermissionFromRoleCommand(id, permissionId));
        return NoContent();
    }
}

using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Identity.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var items = await _mediator.Send(new GetUsersQuery(skip, take));
        return this.OkWithData(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetUserQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpPatch("{id}")]
    [HasPermission("admin.access")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand cmd)
    {
        if (cmd.Id != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpDelete("{id}")]
    [HasPermission("admin.access")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}

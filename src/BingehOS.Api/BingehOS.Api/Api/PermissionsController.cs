using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Identity.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/permissions")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PermissionsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [HasPermission("admin.access")]
    public async Task<IActionResult> Create([FromBody] CreatePermissionCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var items = await _mediator.Send(new GetPermissionsQuery(skip, take));
        return this.OkWithData(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetPermissionQuery(id));
        return this.OkOrNotFound(item);
    }
}

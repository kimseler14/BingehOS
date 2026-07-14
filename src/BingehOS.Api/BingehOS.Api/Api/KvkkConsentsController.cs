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
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id}/revoke")]
    public async Task<IActionResult> Revoke(Guid id, [FromBody] RevokeKvkkConsentCommand cmd)
    {
        if (cmd.Id != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetKvkkConsentQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] Guid? userId = null)
    {
        var items = await _mediator.Send(new GetKvkkConsentsQuery(skip, take, userId));
        return this.OkWithData(items);
    }
}

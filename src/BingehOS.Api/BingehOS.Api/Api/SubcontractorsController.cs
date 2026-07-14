using BingehOS.Modules.Personnel.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/subcontractors")]
[Authorize]
public class SubcontractorsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SubcontractorsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubcontractorCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubcontractorCommand cmd)
    {
        if (cmd.Id != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetSubcontractorQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] bool? activeOnly = null)
    {
        var items = await _mediator.Send(new GetSubcontractorsQuery(skip, take, activeOnly));
        return this.OkWithData(items);
    }
}

using BingehOS.Modules.Facility.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/facilities")]
[Authorize]
public class FacilitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    public FacilitiesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFacilityCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFacilityCommand cmd)
    {
        if (cmd.Id != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetFacilityQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var items = await _mediator.Send(new GetFacilitiesQuery(skip, take));
        return this.OkWithData(items);
    }
}

using BingehOS.Modules.HSE.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/loto-procedures")]
[Authorize]
public class LotoProceduresController : ControllerBase
{
    private readonly IMediator _mediator;
    public LotoProceduresController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLotoProcedureCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id}/verify")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] VerifyLotoProcedureCommand cmd)
    {
        if (cmd.Id != id) return this.IdMismatch();
        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetLotoProcedureQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] Guid? permitToWorkId = null)
    {
        var items = await _mediator.Send(new GetLotoProceduresQuery(skip, take, permitToWorkId));
        return this.OkWithData(items);
    }
}

using BingehOS.Modules.Finance.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/tax-records")]
[Authorize]
public class TaxRecordsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TaxRecordsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaxRecordCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id }, new { success = true, data = new { id } });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetTaxRecordQuery(id));
        if (item == null) return NotFound(new { success = false, error = "not found" });
        return Ok(new { success = true, data = item });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] Guid? invoiceId = null)
    {
        var items = await _mediator.Send(new GetTaxRecordsQuery(skip, take, invoiceId));
        return Ok(new { success = true, data = items });
    }
}

using BingehOS.Modules.HSE.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/risk-assessments")]
[Authorize]
public class RiskAssessmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public RiskAssessmentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRiskAssessmentCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetRiskAssessmentQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] Guid? permitToWorkId = null)
    {
        var items = await _mediator.Send(new GetRiskAssessmentsQuery(skip, take, permitToWorkId));
        return this.OkWithData(items);
    }
}

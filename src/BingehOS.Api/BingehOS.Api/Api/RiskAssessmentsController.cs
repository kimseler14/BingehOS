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
        return CreatedAtAction(nameof(Get), new { id }, new { success = true, data = new { id } });
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id) => Ok(new { success = true, data = new { id } });
}

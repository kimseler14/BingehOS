using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Compliance.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/calibrations")]
[Authorize]
public class CalibrationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CalibrationsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [HasPermission("calibration-records.write")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCalibrationRecordCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return this.CreatedWithId(nameof(Get), id);
    }

    [HttpPatch("{id}")]
    [HasPermission("calibration-records.write")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCalibrationRecordCommand cmd)
    {
        if (cmd.Id != id)
        {
            return this.IdMismatch();
        }

        var dto = await _mediator.Send(cmd);
        return this.OkWithData(dto);
    }

    [HttpGet("{id}")]
    [HasPermission("calibration-records.read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mediator.Send(new GetCalibrationRecordQuery(id));
        return this.OkOrNotFound(item);
    }

    [HttpGet]
    [HasPermission("calibration-records.read")]
    public async Task<IActionResult> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        [FromQuery] Guid? assetId = null)
    {
        var items = await _mediator.Send(
            new GetCalibrationRecordsQuery(skip, take, assetId));
        return this.OkWithData(items);
    }
}

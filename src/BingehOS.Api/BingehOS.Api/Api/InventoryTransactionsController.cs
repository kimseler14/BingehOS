using BingehOS.Infrastructure.Authorization;
using BingehOS.Modules.Inventory.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/inventory/transactions")]
[Authorize]
public class InventoryTransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public InventoryTransactionsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [HasPermission("inventory-transactions.read")]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] Guid? partId = null)
    {
        var items = await _mediator.Send(new GetInventoryTransactionsQuery(skip, take, partId));
        return this.OkWithData(items);
    }
}

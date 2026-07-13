using FacilityOS.Infrastructure;
using MediatR;

namespace FacilityOS.Modules.Finance.Application;

public class CreateWorkOrderCostHandler : IRequestHandler<CreateWorkOrderCostCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateWorkOrderCostHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateWorkOrderCostCommand cmd, CancellationToken ct)
    {
        var cost = Domain.WorkOrderCost.Create(_db.CurrentTenantId, cmd.WorkOrderId, cmd.AmountMinor, cmd.Currency, cmd.Status, cmd.IsApproved);
        _db.Set<Domain.WorkOrderCost>().Add(cost);
        await _db.SaveChangesAsync(ct);
        return cost.Id;
    }
}

using FacilityOS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Modules.Finance.Application;

public class ApproveWorkOrderCostHandler : IRequestHandler<ApproveWorkOrderCostCommand, WorkOrderCostDto>
{
    private readonly AppDbContext _db;
    public ApproveWorkOrderCostHandler(AppDbContext db) => _db = db;

    public async Task<WorkOrderCostDto> Handle(ApproveWorkOrderCostCommand cmd, CancellationToken ct)
    {
        var cost = await _db.Set<Domain.WorkOrderCost>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                    ?? throw new KeyNotFoundException($"WorkOrderCost {cmd.Id} not found.");

        cost.Approve();
        await _db.SaveChangesAsync(ct);
        return new WorkOrderCostDto(cost.Id, cost.WorkOrderId, cost.AmountMinor, cost.Currency, cost.Status, cost.IsApproved);
    }
}

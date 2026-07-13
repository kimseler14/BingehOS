using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public class CreateWorkOrderCostHandler : IRequestHandler<CreateWorkOrderCostCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateWorkOrderCostHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateWorkOrderCostCommand cmd, CancellationToken ct)
    {
        var cost = Domain.WorkOrderCost.Create(_db.CurrentTenantId, cmd.WorkOrderId, cmd.AmountMinor, cmd.Currency, cmd.Status, cmd.IsApproved);
        _db.Set<Domain.WorkOrderCost>().Add(cost);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new WorkOrderCostCreatedEvent(cost.Id, cost.WorkOrderId, cost.AmountMinor), ct);
        return cost.Id;
    }
}

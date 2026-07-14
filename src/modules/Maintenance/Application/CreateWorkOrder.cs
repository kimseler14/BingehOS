// src/modules/Maintenance/Application/CreateWorkOrder.cs
using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Automation.Application;
using BingehOS.Modules.Automation.Domain;
using BingehOS.Modules.Maintenance.Domain;
using BingehOS.Shared.Telemetry;
using MediatR;

namespace BingehOS.Modules.Maintenance.Application;

public class CreateWorkOrderHandler : IRequestHandler<CreateWorkOrderCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;
    private readonly IPublisher _publisher;

    public CreateWorkOrderHandler(AppDbContext db, IEventPublisher eventPublisher, IPublisher publisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateWorkOrderCommand cmd, CancellationToken ct)
    {
        using var activity = BingehOSActivitySource.Source.StartActivity("WorkOrder.Create");
        activity?.SetTag("workorder.asset_id", cmd.AssetId.ToString());
        activity?.SetTag("tenant.id", _db.CurrentTenantId.ToString());

        var wo = WorkOrder.Create(_db.CurrentTenantId, cmd.AssetId, cmd.Description);
        _db.Set<WorkOrder>().Add(wo);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new WorkOrderCreatedEvent(wo.Id, wo.AssetId), ct);
        await _publisher.Publish(new AutomationTriggerNotification(
            _db.CurrentTenantId,
            AutomationTriggerType.WorkOrderCreated,
            new Dictionary<string, object?>
            {
                ["workOrderId"] = wo.Id,
                ["assetId"] = wo.AssetId,
                ["status"] = wo.Status.ToString()
            }), ct);
        return wo.Id;
    }
}

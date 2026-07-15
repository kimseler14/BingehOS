// src/modules/Maintenance/Application/ChangeWorkOrderStatus.cs
using BingehOS.Infrastructure;
using BingehOS.Modules.Automation.Application;
using BingehOS.Modules.Automation.Domain;
using BingehOS.Modules.Maintenance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Maintenance.Application;

public class ChangeWorkOrderStatusHandler : IRequestHandler<ChangeWorkOrderStatusCommand, WorkOrderDto>
{
    private readonly AppDbContext _db;
    private readonly IPublisher _publisher;
    public ChangeWorkOrderStatusHandler(AppDbContext db, IPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<WorkOrderDto> Handle(ChangeWorkOrderStatusCommand cmd, CancellationToken ct)
    {
        var wo = await _db.Set<WorkOrder>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                 ?? throw new KeyNotFoundException($"WorkOrder {cmd.Id} not found.");
        var previousStatus = wo.Status;

        if (cmd.PermitApproved) wo.ApprovePermit();
        if (cmd.ESignatureCaptured) wo.CaptureESignature();

        if (!Enum.TryParse<WorkOrderStatus>(cmd.NewStatus, ignoreCase: true, out var next))
            throw new ArgumentException($"Unknown status {cmd.NewStatus}");

        wo.TransitionTo(next);
        await _db.SaveChangesAsync(ct);
        await _publisher.Publish(new AutomationTriggerNotification(
            _db.CurrentTenantId,
            AutomationTriggerType.WorkOrderStatusChanged,
            new Dictionary<string, object?>
            {
                ["workOrderId"] = wo.Id,
                ["assetId"] = wo.AssetId,
                ["previousStatus"] = previousStatus.ToString(),
                ["status"] = wo.Status.ToString()
            }), ct);
        return new WorkOrderDto(wo.Id, wo.AssetId, wo.Description, wo.Status.ToString(), wo.Priority);
    }
}

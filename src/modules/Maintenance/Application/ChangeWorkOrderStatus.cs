// src/modules/Maintenance/Application/ChangeWorkOrderStatus.cs
using FacilityOS.Infrastructure;
using FacilityOS.Modules.Maintenance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Modules.Maintenance.Application;

public class ChangeWorkOrderStatusHandler : IRequestHandler<ChangeWorkOrderStatusCommand, WorkOrderDto>
{
    private readonly AppDbContext _db;
    public ChangeWorkOrderStatusHandler(AppDbContext db) => _db = db;

    public async Task<WorkOrderDto> Handle(ChangeWorkOrderStatusCommand cmd, CancellationToken ct)
    {
        var wo = await _db.Set<WorkOrder>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                 ?? throw new KeyNotFoundException($"WorkOrder {cmd.Id} not found.");

        if (cmd.PermitApproved) wo.ApprovePermit();
        if (cmd.ESignatureCaptured) wo.CaptureESignature();

        if (!Enum.TryParse<WorkOrderStatus>(cmd.NewStatus, ignoreCase: true, out var next))
            throw new ArgumentException($"Unknown status {cmd.NewStatus}");

        wo.TransitionTo(next);
        await _db.SaveChangesAsync(ct);
        return new WorkOrderDto(wo.Id, wo.AssetId, wo.Description, wo.Status.ToString());
    }
}

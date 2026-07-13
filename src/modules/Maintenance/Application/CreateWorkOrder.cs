// src/modules/Maintenance/Application/CreateWorkOrder.cs
using FacilityOS.Infrastructure;
using FacilityOS.Modules.Maintenance.Domain;
using MediatR;

namespace FacilityOS.Modules.Maintenance.Application;

public class CreateWorkOrderHandler : IRequestHandler<CreateWorkOrderCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateWorkOrderHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateWorkOrderCommand cmd, CancellationToken ct)
    {
        var wo = WorkOrder.Create(_db.CurrentTenantId, cmd.AssetId, cmd.Description);
        _db.Set<WorkOrder>().Add(wo);
        await _db.SaveChangesAsync(ct);
        return wo.Id;
    }
}

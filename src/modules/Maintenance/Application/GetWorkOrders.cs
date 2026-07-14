using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Maintenance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Maintenance.Application;

public record WorkOrderListItem(Guid Id, Guid AssetId, string Description, string Status);

public record GetWorkOrderQuery(Guid Id) : IRequest<WorkOrderListItem?>;
public record GetWorkOrdersQuery(int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<WorkOrderListItem>>;

public class GetWorkOrderHandler : IRequestHandler<GetWorkOrderQuery, WorkOrderListItem?>
{
    private readonly AppDbContext _db;
    public GetWorkOrderHandler(AppDbContext db) => _db = db;

    public async Task<WorkOrderListItem?> Handle(GetWorkOrderQuery q, CancellationToken ct)
    {
        var wo = await _db.Set<WorkOrder>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (wo == null) return null;
        return new WorkOrderListItem(wo.Id, wo.AssetId, wo.Description, wo.Status.ToString());
    }
}

public class GetWorkOrdersHandler : IRequestHandler<GetWorkOrdersQuery, IReadOnlyList<WorkOrderListItem>>
{
    private readonly AppDbContext _db;
    public GetWorkOrdersHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<WorkOrderListItem>> Handle(GetWorkOrdersQuery q, CancellationToken ct)
    {
        return await _db.Set<WorkOrder>()
            .OrderByDescending(e => e.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .Select(e => new WorkOrderListItem(e.Id, e.AssetId, e.Description, e.Status.ToString()))
            .ToListAsync(ct);
    }
}

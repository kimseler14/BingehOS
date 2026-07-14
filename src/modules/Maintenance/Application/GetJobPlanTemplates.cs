using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Maintenance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Maintenance.Application;

public record JobPlanTemplateListItem(Guid Id, string Name, string AssetType, int? EstimatedDurationMinutes, string RequiredPermitType);

public record GetJobPlanTemplateQuery(Guid Id) : IRequest<JobPlanTemplateListItem?>;
public record GetJobPlanTemplatesQuery(int Skip = 0, int Take = 20, string? assetType = null) : IRequest<IReadOnlyList<JobPlanTemplateListItem>>;

public class GetJobPlanTemplateHandler : IRequestHandler<GetJobPlanTemplateQuery, JobPlanTemplateListItem?>
{
    private readonly AppDbContext _db;
    public GetJobPlanTemplateHandler(AppDbContext db) => _db = db;

    public async Task<JobPlanTemplateListItem?> Handle(GetJobPlanTemplateQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<JobPlanTemplate>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new JobPlanTemplateListItem(entity.Id, entity.Name, entity.AssetType, entity.EstimatedDurationMinutes, entity.RequiredPermitType);
    }
}

public class GetJobPlanTemplatesHandler : IRequestHandler<GetJobPlanTemplatesQuery, IReadOnlyList<JobPlanTemplateListItem>>
{
    private readonly AppDbContext _db;
    public GetJobPlanTemplatesHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<JobPlanTemplateListItem>> Handle(GetJobPlanTemplatesQuery q, CancellationToken ct)
    {
        var query = _db.Set<JobPlanTemplate>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.assetType)) query = query.Where(e => e.AssetType == q.assetType);

        return await query.OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new JobPlanTemplateListItem(e.Id, e.Name, e.AssetType, e.EstimatedDurationMinutes, e.RequiredPermitType))
            .ToListAsync(ct);
    }
}

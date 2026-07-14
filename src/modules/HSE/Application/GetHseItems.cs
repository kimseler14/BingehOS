using BingehOS.Infrastructure;
using BingehOS.Modules.HSE.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.HSE.Application;

public record PermitToWorkListItem(Guid Id, string Title, string Status, Guid? FacilityId, Guid? WorkOrderId, Guid? ApproverId);
public record LotoProcedureListItem(Guid Id, string Steps, bool IsVerified, Guid? VerifiedBy, DateTimeOffset? VerifiedAt, Guid PermitToWorkId);
public record RiskAssessmentListItem(Guid Id, string Title, string Level, Guid PermitToWorkId);

public record GetPermitToWorkQuery(Guid Id) : IRequest<PermitToWorkListItem?>;
public record GetPermitsToWorkQuery(int Skip = 0, int Take = 20, string? status = null) : IRequest<IReadOnlyList<PermitToWorkListItem>>;

public record GetLotoProcedureQuery(Guid Id) : IRequest<LotoProcedureListItem?>;
public record GetLotoProceduresQuery(int Skip = 0, int Take = 20, Guid? permitToWorkId = null) : IRequest<IReadOnlyList<LotoProcedureListItem>>;

public record GetRiskAssessmentQuery(Guid Id) : IRequest<RiskAssessmentListItem?>;
public record GetRiskAssessmentsQuery(int Skip = 0, int Take = 20, Guid? permitToWorkId = null) : IRequest<IReadOnlyList<RiskAssessmentListItem>>;

public class GetPermitToWorkHandler : IRequestHandler<GetPermitToWorkQuery, PermitToWorkListItem?>
{
    private readonly AppDbContext _db;
    public GetPermitToWorkHandler(AppDbContext db) => _db = db;

    public async Task<PermitToWorkListItem?> Handle(GetPermitToWorkQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<PermitToWork>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new PermitToWorkListItem(entity.Id, entity.Title, entity.Status, entity.FacilityId, entity.WorkOrderId, entity.ApproverId);
    }
}

public class GetPermitsToWorkHandler : IRequestHandler<GetPermitsToWorkQuery, IReadOnlyList<PermitToWorkListItem>>
{
    private readonly AppDbContext _db;
    public GetPermitsToWorkHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PermitToWorkListItem>> Handle(GetPermitsToWorkQuery q, CancellationToken ct)
    {
        var take = q.Take <= 0 ? 20 : q.Take;
        var skip = q.Skip < 0 ? 0 : q.Skip;
        var query = _db.Set<PermitToWork>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.status)) query = query.Where(e => e.Status == q.status);

        return await query.OrderByDescending(e => e.CreatedAt).Skip(skip).Take(take)
            .Select(e => new PermitToWorkListItem(e.Id, e.Title, e.Status, e.FacilityId, e.WorkOrderId, e.ApproverId))
            .ToListAsync(ct);
    }
}

public class GetLotoProcedureHandler : IRequestHandler<GetLotoProcedureQuery, LotoProcedureListItem?>
{
    private readonly AppDbContext _db;
    public GetLotoProcedureHandler(AppDbContext db) => _db = db;

    public async Task<LotoProcedureListItem?> Handle(GetLotoProcedureQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<LotoProcedure>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new LotoProcedureListItem(entity.Id, entity.Steps, entity.IsVerified, entity.VerifiedBy, entity.VerifiedAt, entity.PermitToWorkId);
    }
}

public class GetLotoProceduresHandler : IRequestHandler<GetLotoProceduresQuery, IReadOnlyList<LotoProcedureListItem>>
{
    private readonly AppDbContext _db;
    public GetLotoProceduresHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<LotoProcedureListItem>> Handle(GetLotoProceduresQuery q, CancellationToken ct)
    {
        var take = q.Take <= 0 ? 20 : q.Take;
        var skip = q.Skip < 0 ? 0 : q.Skip;
        var query = _db.Set<LotoProcedure>().AsQueryable();
        if (q.permitToWorkId.HasValue) query = query.Where(e => e.PermitToWorkId == q.permitToWorkId.Value);

        return await query.OrderByDescending(e => e.CreatedAt).Skip(skip).Take(take)
            .Select(e => new LotoProcedureListItem(e.Id, e.Steps, e.IsVerified, e.VerifiedBy, e.VerifiedAt, e.PermitToWorkId))
            .ToListAsync(ct);
    }
}

public class GetRiskAssessmentHandler : IRequestHandler<GetRiskAssessmentQuery, RiskAssessmentListItem?>
{
    private readonly AppDbContext _db;
    public GetRiskAssessmentHandler(AppDbContext db) => _db = db;

    public async Task<RiskAssessmentListItem?> Handle(GetRiskAssessmentQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<RiskAssessment>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new RiskAssessmentListItem(entity.Id, entity.Title, entity.Level, entity.PermitToWorkId);
    }
}

public class GetRiskAssessmentsHandler : IRequestHandler<GetRiskAssessmentsQuery, IReadOnlyList<RiskAssessmentListItem>>
{
    private readonly AppDbContext _db;
    public GetRiskAssessmentsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<RiskAssessmentListItem>> Handle(GetRiskAssessmentsQuery q, CancellationToken ct)
    {
        var take = q.Take <= 0 ? 20 : q.Take;
        var skip = q.Skip < 0 ? 0 : q.Skip;
        var query = _db.Set<RiskAssessment>().AsQueryable();
        if (q.permitToWorkId.HasValue) query = query.Where(e => e.PermitToWorkId == q.permitToWorkId.Value);

        return await query.OrderByDescending(e => e.CreatedAt).Skip(skip).Take(take)
            .Select(e => new RiskAssessmentListItem(e.Id, e.Title, e.Level, e.PermitToWorkId))
            .ToListAsync(ct);
    }
}

using BingehOS.Infrastructure;
using BingehOS.Modules.Compliance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Compliance.Application;

public record ComplianceRecordListItem(Guid Id, string Title, string Status, DateTime DueDate);
public record KvkkConsentListItem(Guid Id, Guid UserId, string ConsentType, string Version, DateTimeOffset GrantedAt, DateTimeOffset? RevokedAt);

public record GetComplianceRecordQuery(Guid Id) : IRequest<ComplianceRecordListItem?>;
public record GetComplianceRecordsQuery(int Skip = 0, int Take = 20, string? status = null) : IRequest<IReadOnlyList<ComplianceRecordListItem>>;

public record GetKvkkConsentQuery(Guid Id) : IRequest<KvkkConsentListItem?>;
public record GetKvkkConsentsQuery(int Skip = 0, int Take = 20, Guid? userId = null) : IRequest<IReadOnlyList<KvkkConsentListItem>>;

public class GetComplianceRecordHandler : IRequestHandler<GetComplianceRecordQuery, ComplianceRecordListItem?>
{
    private readonly AppDbContext _db;
    public GetComplianceRecordHandler(AppDbContext db) => _db = db;

    public async Task<ComplianceRecordListItem?> Handle(GetComplianceRecordQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<ComplianceRecord>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new ComplianceRecordListItem(entity.Id, entity.Title, entity.Status, entity.DueDate);
    }
}

public class GetComplianceRecordsHandler : IRequestHandler<GetComplianceRecordsQuery, IReadOnlyList<ComplianceRecordListItem>>
{
    private readonly AppDbContext _db;
    public GetComplianceRecordsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ComplianceRecordListItem>> Handle(GetComplianceRecordsQuery q, CancellationToken ct)
    {
        var take = q.Take <= 0 ? 20 : q.Take;
        var skip = q.Skip < 0 ? 0 : q.Skip;
        var query = _db.Set<ComplianceRecord>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.status)) query = query.Where(e => e.Status == q.status);

        return await query.OrderByDescending(e => e.CreatedAt).Skip(skip).Take(take)
            .Select(e => new ComplianceRecordListItem(e.Id, e.Title, e.Status, e.DueDate))
            .ToListAsync(ct);
    }
}

public class GetKvkkConsentHandler : IRequestHandler<GetKvkkConsentQuery, KvkkConsentListItem?>
{
    private readonly AppDbContext _db;
    public GetKvkkConsentHandler(AppDbContext db) => _db = db;

    public async Task<KvkkConsentListItem?> Handle(GetKvkkConsentQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<KvkkConsent>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new KvkkConsentListItem(entity.Id, entity.UserId, entity.ConsentType, entity.Version, entity.GrantedAt, entity.RevokedAt);
    }
}

public class GetKvkkConsentsHandler : IRequestHandler<GetKvkkConsentsQuery, IReadOnlyList<KvkkConsentListItem>>
{
    private readonly AppDbContext _db;
    public GetKvkkConsentsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<KvkkConsentListItem>> Handle(GetKvkkConsentsQuery q, CancellationToken ct)
    {
        var take = q.Take <= 0 ? 20 : q.Take;
        var skip = q.Skip < 0 ? 0 : q.Skip;
        var query = _db.Set<KvkkConsent>().AsQueryable();
        if (q.userId.HasValue) query = query.Where(e => e.UserId == q.userId.Value);

        return await query.OrderByDescending(e => e.CreatedAt).Skip(skip).Take(take)
            .Select(e => new KvkkConsentListItem(e.Id, e.UserId, e.ConsentType, e.Version, e.GrantedAt, e.RevokedAt))
            .ToListAsync(ct);
    }
}

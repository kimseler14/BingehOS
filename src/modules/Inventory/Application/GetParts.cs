using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Inventory.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Inventory.Application;

public record PartListItem(Guid Id, string PartNumber, string Name, string UnitOfMeasure, bool IsActive);

public record GetPartQuery(Guid Id) : IRequest<PartListItem?>;
public record GetPartsQuery(int Skip = 0, int Take = 20, bool? activeOnly = null) : IRequest<IReadOnlyList<PartListItem>>;

public class GetPartHandler : IRequestHandler<GetPartQuery, PartListItem?>
{
    private readonly AppDbContext _db;
    public GetPartHandler(AppDbContext db) => _db = db;

    public async Task<PartListItem?> Handle(GetPartQuery q, CancellationToken ct)
    {
        var part = await _db.Set<Part>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (part == null) return null;
        return new PartListItem(part.Id, part.PartNumber, part.Name, part.UnitOfMeasure, part.IsActive);
    }
}

public class GetPartsHandler : IRequestHandler<GetPartsQuery, IReadOnlyList<PartListItem>>
{
    private readonly AppDbContext _db;
    public GetPartsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PartListItem>> Handle(GetPartsQuery q, CancellationToken ct)
    {
        var query = _db.Set<Part>().AsQueryable();

        if (q.activeOnly.HasValue) query = query.Where(e => e.IsActive == q.activeOnly.Value);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .Select(e => new PartListItem(e.Id, e.PartNumber, e.Name, e.UnitOfMeasure, e.IsActive))
            .ToListAsync(ct);
    }
}

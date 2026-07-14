using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Facility.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Facility.Application;

public record FacilityListItem(Guid Id, string Name, string? Code, string? TimeZone);

public record GetFacilityQuery(Guid Id) : IRequest<FacilityListItem?>;
public record GetFacilitiesQuery(int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<FacilityListItem>>;

public class GetFacilityHandler : IRequestHandler<GetFacilityQuery, FacilityListItem?>
{
    private readonly AppDbContext _db;
    public GetFacilityHandler(AppDbContext db) => _db = db;

    public async Task<FacilityListItem?> Handle(GetFacilityQuery q, CancellationToken ct)
    {
        var facility = await _db.Set<Domain.Facility>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (facility == null) return null;
        return new FacilityListItem(facility.Id, facility.Name, facility.Code, facility.TimeZone);
    }
}

public class GetFacilitiesHandler : IRequestHandler<GetFacilitiesQuery, IReadOnlyList<FacilityListItem>>
{
    private readonly AppDbContext _db;
    public GetFacilitiesHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<FacilityListItem>> Handle(GetFacilitiesQuery q, CancellationToken ct)
    {
        return await _db.Set<Domain.Facility>()
            .OrderByDescending(e => e.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .Select(e => new FacilityListItem(e.Id, e.Name, e.Code, e.TimeZone))
            .ToListAsync(ct);
    }
}

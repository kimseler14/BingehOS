using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Vendor.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Vendor.Application;

public record VendorListItem(Guid Id, string Name, string? ContactEmail, string? Phone, bool IsActive);

public record GetVendorQuery(Guid Id) : IRequest<VendorListItem?>;
public record GetVendorsQuery(int Skip = 0, int Take = 20, bool? activeOnly = null) : IRequest<IReadOnlyList<VendorListItem>>;

public class GetVendorHandler : IRequestHandler<GetVendorQuery, VendorListItem?>
{
    private readonly AppDbContext _db;
    public GetVendorHandler(AppDbContext db) => _db = db;

    public async Task<VendorListItem?> Handle(GetVendorQuery q, CancellationToken ct)
    {
        var vendor = await _db.Set<Domain.Vendor>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (vendor == null) return null;
        return new VendorListItem(vendor.Id, vendor.Name, vendor.ContactEmail, vendor.Phone, vendor.IsActive);
    }
}

public class GetVendorsHandler : IRequestHandler<GetVendorsQuery, IReadOnlyList<VendorListItem>>
{
    private readonly AppDbContext _db;
    public GetVendorsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<VendorListItem>> Handle(GetVendorsQuery q, CancellationToken ct)
    {
        var query = _db.Set<Domain.Vendor>().AsQueryable();

        if (q.activeOnly.HasValue) query = query.Where(e => e.IsActive == q.activeOnly.Value);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .Select(e => new VendorListItem(e.Id, e.Name, e.ContactEmail, e.Phone, e.IsActive))
            .ToListAsync(ct);
    }
}

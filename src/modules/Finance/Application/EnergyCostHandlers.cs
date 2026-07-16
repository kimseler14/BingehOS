using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Finance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Finance.Application;

public record EnergyCostListItem(Guid Id, Guid AssetId, DateTimeOffset BillingPeriodStart, DateTimeOffset BillingPeriodEnd, long AmountMinor, string Currency, string? EnergyType, string? MeterNumber, string? Provider);
public record GetEnergyCostsQuery(Guid? AssetId, int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<EnergyCostListItem>>;

public class GetEnergyCostsHandler : IRequestHandler<GetEnergyCostsQuery, IReadOnlyList<EnergyCostListItem>>
{
    private readonly AppDbContext _db;
    public GetEnergyCostsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<EnergyCostListItem>> Handle(GetEnergyCostsQuery q, CancellationToken ct)
    {
        var query = _db.Set<EnergyCost>().AsQueryable();
        if (q.AssetId.HasValue) query = query.Where(e => e.AssetId == q.AssetId.Value);
        return await query.OrderByDescending(e => e.BillingPeriodEnd).ApplyPaging(q.Skip, q.Take)
            .Select(e => new EnergyCostListItem(e.Id, e.AssetId, e.BillingPeriodStart, e.BillingPeriodEnd, e.AmountMinor, e.Currency, e.EnergyType, e.MeterNumber, e.Provider))
            .ToListAsync(ct);
    }
}

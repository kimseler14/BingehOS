using BingehOS.Infrastructure;
using BingehOS.Modules.Asset.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Asset.Application;

public record AssetListItem(Guid Id, string Name, string? SerialNumber, string? LocationCode, string Criticality);

public record GetAssetQuery(Guid Id) : IRequest<AssetListItem?>;
public record GetAssetsQuery(int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<AssetListItem>>;

public class GetAssetHandler : IRequestHandler<GetAssetQuery, AssetListItem?>
{
    private readonly AppDbContext _db;
    public GetAssetHandler(AppDbContext db) => _db = db;

    public async Task<AssetListItem?> Handle(GetAssetQuery q, CancellationToken ct)
    {
        var asset = await _db.Set<Domain.Asset>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (asset == null) return null;
        return new AssetListItem(asset.Id, asset.Name, asset.SerialNumber, asset.LocationCode, asset.Criticality.ToString());
    }
}

public class GetAssetsHandler : IRequestHandler<GetAssetsQuery, IReadOnlyList<AssetListItem>>
{
    private readonly AppDbContext _db;
    public GetAssetsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<AssetListItem>> Handle(GetAssetsQuery q, CancellationToken ct)
    {
        var take = q.Take <= 0 ? 20 : q.Take;
        var skip = q.Skip < 0 ? 0 : q.Skip;

        return await _db.Set<Domain.Asset>()
            .OrderByDescending(e => e.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(e => new AssetListItem(e.Id, e.Name, e.SerialNumber, e.LocationCode, e.Criticality.ToString()))
            .ToListAsync(ct);
    }
}

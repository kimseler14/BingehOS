using BingehOS.Infrastructure;
using BingehOS.Modules.Asset.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Asset.Application;

public class UpdateAssetHandler : IRequestHandler<UpdateAssetCommand, AssetDto>
{
    private readonly AppDbContext _db;
    public UpdateAssetHandler(AppDbContext db) => _db = db;

    public async Task<AssetDto> Handle(UpdateAssetCommand cmd, CancellationToken ct)
    {
        var asset = await _db.Set<Domain.Asset>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                     ?? throw new KeyNotFoundException($"Asset {cmd.Id} not found.");

        asset.Rename(cmd.Name);
        asset.ChangeLocation(cmd.LocationCode);
        asset.SetCriticality(cmd.Criticality);

        await _db.SaveChangesAsync(ct);
        return new AssetDto(asset.Id, asset.Name, asset.SerialNumber, asset.LocationCode, asset.Criticality.ToString());
    }
}

using BingehOS.Infrastructure;
using BingehOS.Modules.Asset.Domain;
using MediatR;

namespace BingehOS.Modules.Asset.Application;

public class CreateAssetHandler : IRequestHandler<CreateAssetCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateAssetHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateAssetCommand cmd, CancellationToken ct)
    {
        var asset = Domain.Asset.Create(_db.CurrentTenantId, cmd.Name, cmd.SerialNumber, cmd.LocationCode, cmd.Criticality);
        _db.Set<Domain.Asset>().Add(asset);
        await _db.SaveChangesAsync(ct);
        return asset.Id;
    }
}

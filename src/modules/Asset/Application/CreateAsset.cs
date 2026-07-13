using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Asset.Domain;
using MediatR;

namespace BingehOS.Modules.Asset.Application;

public class CreateAssetHandler : IRequestHandler<CreateAssetCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateAssetHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateAssetCommand cmd, CancellationToken ct)
    {
        var asset = Domain.Asset.Create(_db.CurrentTenantId, cmd.Name, cmd.SerialNumber, cmd.LocationCode, cmd.Criticality);
        _db.Set<Domain.Asset>().Add(asset);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new AssetCreatedEvent(asset.Id, asset.Name, asset.SerialNumber), ct);
        return asset.Id;
    }
}

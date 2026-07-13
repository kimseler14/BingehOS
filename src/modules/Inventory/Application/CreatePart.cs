using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Inventory.Domain;
using MediatR;

namespace BingehOS.Modules.Inventory.Application;

public class CreatePartHandler : IRequestHandler<CreatePartCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreatePartHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreatePartCommand cmd, CancellationToken ct)
    {
        var part = Domain.Part.Create(_db.CurrentTenantId, cmd.PartNumber, cmd.Name, cmd.Description, cmd.UnitOfMeasure, cmd.IsActive);
        _db.Set<Domain.Part>().Add(part);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new PartCreatedEvent(part.Id, part.PartNumber, part.Name), ct);
        return part.Id;
    }
}

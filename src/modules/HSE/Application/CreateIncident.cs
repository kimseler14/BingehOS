using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.HSE.Domain;
using MediatR;

namespace BingehOS.Modules.HSE.Application;

public class CreateIncidentHandler : IRequestHandler<CreateIncidentCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateIncidentHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateIncidentCommand cmd, CancellationToken ct)
    {
        var incident = Domain.Incident.Create(_db.CurrentTenantId, cmd.Title, cmd.Description, cmd.Severity, cmd.OccurredAt, cmd.IsResolved);
        _db.Set<Domain.Incident>().Add(incident);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new IncidentCreatedEvent(incident.Id, incident.Title, incident.Severity), ct);
        return incident.Id;
    }
}

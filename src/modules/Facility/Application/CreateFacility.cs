using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Facility.Domain;
using MediatR;

namespace BingehOS.Modules.Facility.Application;

public class CreateFacilityHandler : IRequestHandler<CreateFacilityCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateFacilityHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateFacilityCommand cmd, CancellationToken ct)
    {
        var facility = Domain.Facility.Create(_db.CurrentTenantId, cmd.Name, cmd.Code, cmd.Address, cmd.TimeZone, cmd.ParentFacilityId);
        _db.Set<Domain.Facility>().Add(facility);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new FacilityCreatedEvent(facility.Id, facility.Name, facility.Code), ct);
        return facility.Id;
    }
}

using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.HSE.Domain;
using MediatR;

namespace BingehOS.Modules.HSE.Application;

public class CreatePermitToWorkHandler : IRequestHandler<CreatePermitToWorkCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreatePermitToWorkHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreatePermitToWorkCommand cmd, CancellationToken ct)
    {
        var permit = PermitToWork.Create(_db.CurrentTenantId, cmd.Title, cmd.Description, cmd.FacilityId, cmd.WorkOrderId);
        _db.Set<PermitToWork>().Add(permit);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new PermitToWorkCreatedEvent(permit.Id, permit.Title, permit.Status), ct);
        return permit.Id;
    }
}

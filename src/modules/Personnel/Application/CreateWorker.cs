using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Personnel.Domain;
using MediatR;

namespace BingehOS.Modules.Personnel.Application;

public class CreateWorkerHandler : IRequestHandler<CreateWorkerCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateWorkerHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateWorkerCommand cmd, CancellationToken ct)
    {
        var worker = Domain.Worker.Create(_db.CurrentTenantId, cmd.FirstName, cmd.LastName, cmd.EmployeeNumber, cmd.Department, cmd.Phone, cmd.IsActive);
        _db.Set<Domain.Worker>().Add(worker);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new WorkerCreatedEvent(worker.Id, worker.FirstName, worker.LastName), ct);
        return worker.Id;
    }
}

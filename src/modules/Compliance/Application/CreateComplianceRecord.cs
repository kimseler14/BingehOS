using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Compliance.Domain;
using MediatR;

namespace BingehOS.Modules.Compliance.Application;

public class CreateComplianceRecordHandler : IRequestHandler<CreateComplianceRecordCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateComplianceRecordHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateComplianceRecordCommand cmd, CancellationToken ct)
    {
        var record = Domain.ComplianceRecord.Create(_db.CurrentTenantId, cmd.Title, cmd.Description, cmd.Status, cmd.DueDate);
        _db.Set<Domain.ComplianceRecord>().Add(record);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new ComplianceRecordCreatedEvent(record.Id, record.Title), ct);
        return record.Id;
    }
}

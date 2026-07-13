using FacilityOS.Infrastructure;
using MediatR;

namespace FacilityOS.Modules.Compliance.Application;

public class CreateComplianceRecordHandler : IRequestHandler<CreateComplianceRecordCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateComplianceRecordHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateComplianceRecordCommand cmd, CancellationToken ct)
    {
        var record = Domain.ComplianceRecord.Create(_db.CurrentTenantId, cmd.Title, cmd.Description, cmd.Status, cmd.DueDate);
        _db.Set<Domain.ComplianceRecord>().Add(record);
        await _db.SaveChangesAsync(ct);
        return record.Id;
    }
}

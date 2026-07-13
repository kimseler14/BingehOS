using FacilityOS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Modules.Compliance.Application;

public class UpdateComplianceRecordHandler : IRequestHandler<UpdateComplianceRecordCommand, ComplianceRecordDto>
{
    private readonly AppDbContext _db;
    public UpdateComplianceRecordHandler(AppDbContext db) => _db = db;

    public async Task<ComplianceRecordDto> Handle(UpdateComplianceRecordCommand cmd, CancellationToken ct)
    {
        var record = await _db.Set<Domain.ComplianceRecord>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                      ?? throw new KeyNotFoundException($"ComplianceRecord {cmd.Id} not found.");

        record.Update(cmd.Title, cmd.Description, cmd.Status, cmd.DueDate);
        await _db.SaveChangesAsync(ct);
        return new ComplianceRecordDto(record.Id, record.Title, record.Description, record.Status, record.DueDate);
    }
}

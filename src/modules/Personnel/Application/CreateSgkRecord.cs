using BingehOS.Infrastructure;
using BingehOS.Modules.Personnel.Domain;
using MediatR;

namespace BingehOS.Modules.Personnel.Application;

public class CreateSgkRecordHandler : IRequestHandler<CreateSgkRecordCommand, Guid>
{
    private readonly AppDbContext _db;

    public CreateSgkRecordHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateSgkRecordCommand cmd, CancellationToken ct)
    {
        var record = SgkRecord.Create(_db.CurrentTenantId, cmd.EmployeeId, cmd.SgkNumber, cmd.ProfessionCode, cmd.NaceCode, cmd.RegistrationDate);
        _db.Set<SgkRecord>().Add(record);
        await _db.SaveChangesAsync(ct);
        return record.Id;
    }
}

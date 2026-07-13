using BingehOS.Infrastructure;
using BingehOS.Modules.Personnel.Domain;
using MediatR;

namespace BingehOS.Modules.Personnel.Application;

public class CreateSubcontractorHandler : IRequestHandler<CreateSubcontractorCommand, Guid>
{
    private readonly AppDbContext _db;

    public CreateSubcontractorHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateSubcontractorCommand cmd, CancellationToken ct)
    {
        var subcontractor = Subcontractor.Create(_db.CurrentTenantId, cmd.CompanyName, cmd.TaxNumber, cmd.ContactPerson, cmd.Phone, cmd.IsActive);
        _db.Set<Subcontractor>().Add(subcontractor);
        await _db.SaveChangesAsync(ct);
        return subcontractor.Id;
    }
}

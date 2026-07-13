using BingehOS.Infrastructure;
using BingehOS.Modules.Personnel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Personnel.Application;

public class UpdateSubcontractorHandler : IRequestHandler<UpdateSubcontractorCommand, SubcontractorDto>
{
    private readonly AppDbContext _db;
    public UpdateSubcontractorHandler(AppDbContext db) => _db = db;

    public async Task<SubcontractorDto> Handle(UpdateSubcontractorCommand cmd, CancellationToken ct)
    {
        var subcontractor = await _db.Set<Subcontractor>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                           ?? throw new KeyNotFoundException($"Subcontractor {cmd.Id} not found.");

        subcontractor.Update(cmd.CompanyName, cmd.TaxNumber, cmd.ContactPerson, cmd.Phone);
        if (cmd.IsActive) subcontractor.Activate(); else subcontractor.Deactivate();

        await _db.SaveChangesAsync(ct);
        return new SubcontractorDto(subcontractor.Id, subcontractor.CompanyName, subcontractor.TaxNumber, subcontractor.ContactPerson, subcontractor.Phone, subcontractor.IsActive);
    }
}

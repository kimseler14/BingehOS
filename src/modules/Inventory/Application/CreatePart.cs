using FacilityOS.Infrastructure;
using MediatR;

namespace FacilityOS.Modules.Inventory.Application;

public class CreatePartHandler : IRequestHandler<CreatePartCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreatePartHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreatePartCommand cmd, CancellationToken ct)
    {
        var part = Domain.Part.Create(_db.CurrentTenantId, cmd.PartNumber, cmd.Name, cmd.Description, cmd.UnitOfMeasure, cmd.IsActive);
        _db.Set<Domain.Part>().Add(part);
        await _db.SaveChangesAsync(ct);
        return part.Id;
    }
}

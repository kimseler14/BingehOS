using BingehOS.Infrastructure;
using MediatR;

namespace BingehOS.Modules.Facility.Application;

public class CreateFacilityHandler : IRequestHandler<CreateFacilityCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateFacilityHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateFacilityCommand cmd, CancellationToken ct)
    {
        var facility = Domain.Facility.Create(_db.CurrentTenantId, cmd.Name, cmd.Code, cmd.Address, cmd.TimeZone, cmd.ParentFacilityId);
        _db.Set<Domain.Facility>().Add(facility);
        await _db.SaveChangesAsync(ct);
        return facility.Id;
    }
}

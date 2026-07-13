using FacilityOS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Modules.Facility.Application;

public class UpdateFacilityHandler : IRequestHandler<UpdateFacilityCommand, FacilityDto>
{
    private readonly AppDbContext _db;
    public UpdateFacilityHandler(AppDbContext db) => _db = db;

    public async Task<FacilityDto> Handle(UpdateFacilityCommand cmd, CancellationToken ct)
    {
        var facility = await _db.Set<Domain.Facility>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                       ?? throw new KeyNotFoundException($"Facility {cmd.Id} not found.");

        facility.Rename(cmd.Name);
        facility.ChangeAddress(cmd.Address);
        facility.SetTimeZone(cmd.TimeZone);

        await _db.SaveChangesAsync(ct);
        return new FacilityDto(facility.Id, facility.Name, facility.Code, facility.Address, facility.TimeZone, facility.ParentFacilityId);
    }
}

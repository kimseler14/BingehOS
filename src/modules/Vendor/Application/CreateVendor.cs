using FacilityOS.Infrastructure;
using MediatR;

namespace FacilityOS.Modules.Vendor.Application;

public class CreateVendorHandler : IRequestHandler<CreateVendorCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateVendorHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateVendorCommand cmd, CancellationToken ct)
    {
        var vendor = Domain.Vendor.Create(_db.CurrentTenantId, cmd.Name, cmd.TaxNumber, cmd.ContactEmail, cmd.Phone, cmd.IsActive);
        _db.Set<Domain.Vendor>().Add(vendor);
        await _db.SaveChangesAsync(ct);
        return vendor.Id;
    }
}

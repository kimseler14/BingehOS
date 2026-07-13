using FacilityOS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Modules.Vendor.Application;

public class UpdateVendorHandler : IRequestHandler<UpdateVendorCommand, VendorDto>
{
    private readonly AppDbContext _db;
    public UpdateVendorHandler(AppDbContext db) => _db = db;

    public async Task<VendorDto> Handle(UpdateVendorCommand cmd, CancellationToken ct)
    {
        var vendor = await _db.Set<Domain.Vendor>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                      ?? throw new KeyNotFoundException($"Vendor {cmd.Id} not found.");

        vendor.Rename(cmd.Name);
        if (cmd.IsActive) vendor.Activate(); else vendor.Deactivate();

        await _db.SaveChangesAsync(ct);
        return new VendorDto(vendor.Id, vendor.Name, vendor.TaxNumber, vendor.ContactEmail, vendor.Phone, vendor.IsActive);
    }
}

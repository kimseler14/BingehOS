using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Vendor.Domain;
using MediatR;

namespace BingehOS.Modules.Vendor.Application;

public class CreateVendorHandler : IRequestHandler<CreateVendorCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateVendorHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateVendorCommand cmd, CancellationToken ct)
    {
        var vendor = Domain.Vendor.Create(_db.CurrentTenantId, cmd.Name, cmd.TaxNumber, cmd.ContactEmail, cmd.Phone, cmd.IsActive);
        _db.Set<Domain.Vendor>().Add(vendor);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new VendorCreatedEvent(vendor.Id, vendor.Name), ct);
        return vendor.Id;
    }
}

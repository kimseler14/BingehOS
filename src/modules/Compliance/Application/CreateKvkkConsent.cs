using BingehOS.Infrastructure;
using BingehOS.Modules.Compliance.Domain;
using MediatR;

namespace BingehOS.Modules.Compliance.Application;

public class CreateKvkkConsentHandler : IRequestHandler<CreateKvkkConsentCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateKvkkConsentHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateKvkkConsentCommand cmd, CancellationToken ct)
    {
        var consent = KvkkConsent.Create(_db.CurrentTenantId, cmd.UserId, cmd.ConsentType, cmd.Version, cmd.IpAddress, cmd.SignatureHash);
        _db.Set<KvkkConsent>().Add(consent);
        await _db.SaveChangesAsync(ct);
        return consent.Id;
    }
}

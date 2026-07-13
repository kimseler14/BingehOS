using BingehOS.Infrastructure;
using BingehOS.Modules.Compliance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Compliance.Application;

public class RevokeKvkkConsentHandler : IRequestHandler<RevokeKvkkConsentCommand, KvkkConsentDto>
{
    private readonly AppDbContext _db;
    public RevokeKvkkConsentHandler(AppDbContext db) => _db = db;

    public async Task<KvkkConsentDto> Handle(RevokeKvkkConsentCommand cmd, CancellationToken ct)
    {
        var consent = await _db.Set<KvkkConsent>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                     ?? throw new KeyNotFoundException($"KvkkConsent {cmd.Id} not found.");

        consent.Revoke(cmd.IpAddress, cmd.SignatureHash);
        await _db.SaveChangesAsync(ct);
        return new KvkkConsentDto(consent.Id, consent.UserId, consent.ConsentType, consent.Version, consent.GrantedAt, consent.RevokedAt);
    }
}

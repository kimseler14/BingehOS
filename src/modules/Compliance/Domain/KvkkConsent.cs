using BingehOS.Shared;

namespace BingehOS.Modules.Compliance.Domain;

public class KvkkConsent : BaseEntity
{
    public Guid UserId { get; private set; }
    public string ConsentType { get; private set; } = string.Empty;
    public string Version { get; private set; } = string.Empty;
    public DateTimeOffset GrantedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string SignatureHash { get; private set; } = string.Empty;

    public static KvkkConsent Create(Guid tenantId, Guid userId, string consentType, string version, string ipAddress, string signatureHash)
        => new() { TenantId = tenantId, UserId = userId, ConsentType = consentType, Version = version, GrantedAt = DateTimeOffset.UtcNow, IpAddress = ipAddress, SignatureHash = signatureHash };

    public void Revoke(string ipAddress, string signatureHash)
    {
        RevokedAt = DateTimeOffset.UtcNow;
        IpAddress = ipAddress;
        SignatureHash = signatureHash;
    }
}

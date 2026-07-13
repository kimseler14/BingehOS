using BingehOS.Modules.Compliance.Domain;
using Xunit;

namespace BingehOS.UnitTests;

public class KvkkConsentTests
{
    [Fact]
    public void Create_SetsProperties()
    {
        var consent = KvkkConsent.Create(Guid.NewGuid(), Guid.NewGuid(), "PrivacyNotice", "v1", "127.0.0.1", "hash123");
        Assert.Equal("PrivacyNotice", consent.ConsentType);
        Assert.Equal("v1", consent.Version);
        Assert.Equal("127.0.0.1", consent.IpAddress);
        Assert.Null(consent.RevokedAt);
    }

    [Fact]
    public void Revoke_SetsRevokedAt()
    {
        var consent = KvkkConsent.Create(Guid.NewGuid(), Guid.NewGuid(), "DataProcessing", "v1", "10.0.0.1", "hash");
        consent.Revoke("10.0.0.1", "hash-revoke");
        Assert.NotNull(consent.RevokedAt);
    }
}

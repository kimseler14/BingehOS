using BingehOS.Modules.Plugin.Domain;
using Xunit;

namespace BingehOS.UnitTests;

public class PluginRegistrationTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Register_StartsAvailable_AndEnableSetsInstalledAt()
    {
        var plugin = PluginRegistration.Register(
            TenantId,
            "Bakım raporları",
            "1.0.0",
            "Raporlama eklentisi",
            "BingehOS",
            "https://example.test/plugin.zip",
            "/plugins/reports");

        Assert.Equal(TenantId, plugin.TenantId);
        Assert.Equal(PluginStatus.Available, plugin.Status);
        Assert.Null(plugin.InstalledAt);

        plugin.SetStatus(PluginStatus.Enabled);

        Assert.Equal(PluginStatus.Enabled, plugin.Status);
        Assert.NotNull(plugin.InstalledAt);
    }

    [Fact]
    public void SoftDelete_DisablesAndHidesPlugin()
    {
        var plugin = PluginRegistration.Register(
            TenantId,
            "Eklenti",
            "1.0.0",
            null,
            null,
            null,
            null);

        plugin.SoftDelete();

        Assert.True(plugin.IsDeleted);
        Assert.Equal(PluginStatus.Disabled, plugin.Status);
    }
}

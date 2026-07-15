using BingehOS.Infrastructure.Plugins;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BingehOS.UnitTests;

public class PluginLoaderTests
{
    [Fact]
    public async Task LoadAsync_NonExistentDirectory_DoesNotThrow()
    {
        var loader = new PluginLoader(NullLogger<PluginLoader>.Instance);
        var ex = await Record.ExceptionAsync(() => loader.LoadAsync("/nonexistent/plugins/dir"));
        Assert.Null(ex);
    }

    [Fact]
    public async Task LoadAsync_EmptyDirectory_ReturnsNoPlugins()
    {
        var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
        System.IO.Directory.CreateDirectory(tempDir);

        try
        {
            var loader = new PluginLoader(NullLogger<PluginLoader>.Instance);
            await loader.LoadAsync(tempDir);

            Assert.Empty(loader.Plugins);
        }
        finally
        {
            System.IO.Directory.Delete(tempDir);
        }
    }

    [Fact]
    public async Task LoadAsync_Canceled_PropagatesCancellation()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var loader = new PluginLoader(NullLogger<PluginLoader>.Instance);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => loader.LoadAsync("/nonexistent/plugins/dir", cts.Token));
    }
}

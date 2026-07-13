using BingehOS.Infrastructure.Plugins;
using Xunit;

namespace BingehOS.UnitTests;

public class PluginLoaderTests
{
    [Fact]
    public async Task LoadAsync_NonExistentDirectory_DoesNotThrow()
    {
        var loader = new PluginLoader();
        var ex = await Record.ExceptionAsync(() => loader.LoadAsync("/nonexistent/plugins/dir"));
        Assert.Null(ex);
    }

    [Fact]
    public async Task LoadAsync_EmptyDirectory_ReturnsNoPlugins()
    {
        var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
        System.IO.Directory.CreateDirectory(tempDir);

        var loader = new PluginLoader();
        await loader.LoadAsync(tempDir);

        Assert.Empty(loader.Plugins);

        System.IO.Directory.Delete(tempDir);
    }
}

using BingehOS.Infrastructure.Plugins;
using Microsoft.Extensions.Hosting;

namespace BingehOS.Infrastructure;

public class PluginLoaderHostedService : BackgroundService
{
    private readonly PluginLoader _pluginLoader;
    private readonly string _pluginsDirectory;

    public PluginLoaderHostedService(PluginLoader pluginLoader, IHostEnvironment env)
    {
        _pluginLoader = pluginLoader;
        _pluginsDirectory = Path.Combine(env.ContentRootPath, "plugins");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _pluginLoader.LoadAsync(_pluginsDirectory, stoppingToken);
    }
}

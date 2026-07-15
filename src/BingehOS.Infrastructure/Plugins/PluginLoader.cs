using System.Reflection;
using Microsoft.Extensions.Logging;

namespace BingehOS.Infrastructure.Plugins;

public class PluginLoader
{
    private readonly List<IPlugin> _plugins = new();
    private readonly ILogger<PluginLoader> _logger;

    public PluginLoader(ILogger<PluginLoader> logger) => _logger = logger;

    public IReadOnlyList<IPlugin> Plugins => _plugins;

    public async Task LoadAsync(string pluginsDirectory, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (!Directory.Exists(pluginsDirectory))
            return;

        foreach (var dll in Directory.EnumerateFiles(pluginsDirectory, "*.dll"))
        {
            ct.ThrowIfCancellationRequested();

            IEnumerable<Type> pluginTypes;

            try
            {
                var assembly = Assembly.LoadFrom(dll);
                pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load plugin assembly {PluginAssembly}", dll);
                continue;
            }

            foreach (var type in pluginTypes)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var plugin = Activator.CreateInstance(type) as IPlugin
                        ?? throw new InvalidOperationException($"Could not create plugin type {type.FullName}.");

                    await plugin.InitializeAsync(ct);
                    _plugins.Add(plugin);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to initialize plugin {PluginType} from {PluginAssembly}",
                        type.FullName,
                        dll);
                }
            }
        }
    }
}

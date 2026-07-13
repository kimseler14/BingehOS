using System.Reflection;

namespace BingehOS.Infrastructure.Plugins;

public class PluginLoader
{
    private readonly List<IPlugin> _plugins = new();
    public IReadOnlyList<IPlugin> Plugins => _plugins;

    public async Task LoadAsync(string pluginsDirectory, CancellationToken ct = default)
    {
        if (!Directory.Exists(pluginsDirectory))
            return;

        foreach (var dll in Directory.EnumerateFiles(pluginsDirectory, "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(dll);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var type in pluginTypes)
                {
                    if (Activator.CreateInstance(type) is IPlugin plugin)
                    {
                        await plugin.InitializeAsync(ct);
                        _plugins.Add(plugin);
                    }
                }
            }
            catch
            {
            }
        }
    }
}

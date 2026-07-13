namespace BingehOS.Infrastructure.Plugins;

public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    Task InitializeAsync(CancellationToken ct = default);
}

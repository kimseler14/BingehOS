using BingehOS.Infrastructure.Plugins;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/plugins")]
[Authorize]
public class PluginsController : ControllerBase
{
    private readonly PluginLoader _pluginLoader;
    public PluginsController(PluginLoader pluginLoader) => _pluginLoader = pluginLoader;

    [HttpGet]
    public IActionResult GetAll()
    {
        var plugins = _pluginLoader.Plugins.Select(p => new { p.Name, p.Version });
        return Ok(new { success = true, data = plugins });
    }
}

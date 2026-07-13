using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.HSE.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class IncidentsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public IncidentsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/incidents",
            new CreateIncidentCommand("Slip", "Wet floor", "Medium", DateTime.UtcNow, false));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/incidents/{id}",
            new UpdateIncidentCommand(id, "Slip Updated", "Wet floor", "Medium", true));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

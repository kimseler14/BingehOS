using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FacilityOS.Modules.Inventory.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FacilityOS.IntegrationTests;

public class PartsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public PartsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/parts",
            new CreatePartCommand("PN-1", "Bearing", "6001-2RS", "pcs", true));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/parts/{id}",
            new UpdatePartCommand(id, "Bearing V2", "6001-2RS", "pcs", false));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

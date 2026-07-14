using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Asset.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class AssetEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public AssetEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync("/v1/assets",
            new CreateAssetCommand("Pump A", "SN-1", "B1", BingehOS.Modules.Asset.Domain.AssetCriticality.High));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/assets/{id}",
            new UpdateAssetCommand(id, "Pump A Renamed", "B2", BingehOS.Modules.Asset.Domain.AssetCriticality.Critical));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

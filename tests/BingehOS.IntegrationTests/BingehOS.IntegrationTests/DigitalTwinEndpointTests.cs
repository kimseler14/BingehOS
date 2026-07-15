using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Asset.Application;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.DigitalTwin.Application;
using Xunit;

namespace BingehOS.IntegrationTests;

public class DigitalTwinEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;

    public DigitalTwinEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task FloorPlanCrud_And_PositionRoundTrip_AreSupported()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var assetResponse = await client.PostAsJsonAsync(
            "/v1/assets",
            new CreateAssetCommand("Dijital ikiz varlığı", "DT-01", "Z-1", AssetCriticality.Normal));
        Assert.Equal(HttpStatusCode.Created, assetResponse.StatusCode);
        var assetBody = await assetResponse.Content.ReadFromJsonAsync<JsonElement>();
        var assetId = assetBody.GetProperty("data").GetProperty("id").GetGuid();

        var create = await client.PostAsJsonAsync(
            "/v1/floor-plans",
            new CreateFloorPlanCommand("Zemin Kat Planı", null, null, 1200, 800));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var createBody = await create.Content.ReadFromJsonAsync<JsonElement>();
        var floorPlanId = createBody.GetProperty("data").GetProperty("id").GetGuid();

        var replace = await client.PutAsJsonAsync(
            $"/v1/floor-plans/{floorPlanId}/positions",
            new
            {
                positions = new[]
                {
                    new { assetId, x = 0.25, y = 0.75 }
                }
            });
        Assert.Equal(HttpStatusCode.OK, replace.StatusCode);

        var positions = await client.GetAsync($"/v1/floor-plans/{floorPlanId}/positions");
        Assert.Equal(HttpStatusCode.OK, positions.StatusCode);
        var positionsBody = await positions.Content.ReadFromJsonAsync<JsonElement>();
        var position = Assert.Single(positionsBody.GetProperty("data").EnumerateArray());
        Assert.Equal(assetId, position.GetProperty("assetId").GetGuid());
        Assert.Equal(0.25, position.GetProperty("x").GetDouble());
        Assert.Equal(0.75, position.GetProperty("y").GetDouble());

        var deleted = await client.DeleteAsync($"/v1/floor-plans/{floorPlanId}");
        Assert.Equal(HttpStatusCode.OK, deleted.StatusCode);
        var getDeleted = await client.GetAsync($"/v1/floor-plans/{floorPlanId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeleted.StatusCode);
    }
}

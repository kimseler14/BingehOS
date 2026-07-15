using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Asset.Application;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.Inventory.Application;
using BingehOS.Modules.Maintenance.Application;
using Xunit;

namespace BingehOS.IntegrationTests;

public class InsightsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;

    public InsightsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task AssetAndPartInsights_ReturnComputedResults()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var assetResponse = await client.PostAsJsonAsync(
            "/v1/assets",
            new CreateAssetCommand("İçgörü Kompresörü", "AI-01", "A-1", AssetCriticality.High));
        Assert.Equal(HttpStatusCode.Created, assetResponse.StatusCode);
        var assetBody = await assetResponse.Content.ReadFromJsonAsync<JsonElement>();
        var assetId = assetBody.GetProperty("data").GetProperty("id").GetGuid();

        var workOrder = await client.PostAsJsonAsync(
            "/v1/work-orders",
            new CreateWorkOrderCommand(assetId, "Acil arıza onarımı"));
        Assert.Equal(HttpStatusCode.Created, workOrder.StatusCode);

        var partResponse = await client.PostAsJsonAsync(
            "/v1/parts",
            new CreatePartCommand("AI-FLT", "İçgörü filtresi", null, "pcs", true));
        Assert.Equal(HttpStatusCode.Created, partResponse.StatusCode);
        var partBody = await partResponse.Content.ReadFromJsonAsync<JsonElement>();
        var partId = partBody.GetProperty("data").GetProperty("id").GetGuid();

        var receive = await client.PostAsJsonAsync($"/v1/parts/{partId}/receive", new { partId, quantity = 2 });
        Assert.Equal(HttpStatusCode.OK, receive.StatusCode);
        var issue = await client.PostAsJsonAsync($"/v1/parts/{partId}/issue", new { partId, quantity = 2 });
        Assert.Equal(HttpStatusCode.OK, issue.StatusCode);

        var assets = await client.GetAsync("/v1/insights/assets");
        Assert.Equal(HttpStatusCode.OK, assets.StatusCode);
        var assetInsights = await assets.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(
            assetInsights.GetProperty("data").EnumerateArray(),
            item => item.GetProperty("assetId").GetGuid() == assetId);

        var parts = await client.GetAsync("/v1/insights/parts");
        Assert.Equal(HttpStatusCode.OK, parts.StatusCode);
        var partInsights = await parts.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(
            partInsights.GetProperty("data").EnumerateArray(),
            item => item.GetProperty("partId").GetGuid() == partId);
    }
}

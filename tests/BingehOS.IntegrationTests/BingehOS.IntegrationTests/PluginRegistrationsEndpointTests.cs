using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Plugin.Application;
using BingehOS.Modules.Plugin.Domain;
using Xunit;

namespace BingehOS.IntegrationTests;

public class PluginRegistrationsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;

    public PluginRegistrationsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Register_List_Update_Enable_And_Delete_Are_Supported()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync(
            "/v1/plugins",
            new CreatePluginRegistrationCommand(
                "Raporlama",
                "1.0.0",
                "Bakım raporları",
                "BingehOS",
                "https://example.test/reports.zip",
                "/plugins/reports"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var createBody = await create.Content.ReadFromJsonAsync<JsonElement>();
        var pluginId = createBody.GetProperty("data").GetProperty("id").GetGuid();

        var listed = await client.GetAsync("/v1/plugins?skip=0&take=20");
        Assert.Equal(HttpStatusCode.OK, listed.StatusCode);
        var listBody = await listed.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(
            listBody.GetProperty("data").EnumerateArray(),
            item => item.GetProperty("id").GetGuid() == pluginId);

        var update = await client.PatchAsJsonAsync(
            $"/v1/plugins/{pluginId}",
            new UpdatePluginRegistrationCommand(
                pluginId,
                "Raporlama",
                "1.1.0",
                "Güncel bakım raporları",
                "BingehOS",
                PluginStatus.Enabled,
                "https://example.test/reports.zip",
                "/plugins/reports"));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var updateBody = await update.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Enabled", updateBody.GetProperty("data").GetProperty("status").GetString());

        var deleted = await client.DeleteAsync($"/v1/plugins/{pluginId}");
        Assert.Equal(HttpStatusCode.OK, deleted.StatusCode);

        var getDeleted = await client.GetAsync($"/v1/plugins/{pluginId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeleted.StatusCode);
    }
}

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Personnel.Application;
using Xunit;

namespace BingehOS.IntegrationTests;

public class WorkersEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;

    public WorkersEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_Update_Get_And_List_Worker()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync(
            "/v1/workers",
            new CreateWorkerCommand(
                "Ali",
                "Veli",
                "WRK-1",
                "Electrician",
                "Maintenance",
                "+90 555 000 00 00",
                true));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var createBody = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = createBody.GetProperty("data").GetProperty("id").GetGuid();

        var get = await client.GetAsync($"/v1/workers/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var getBody = await get.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Electrician", getBody.GetProperty("data").GetProperty("trade").GetString());

        var update = await client.PatchAsJsonAsync(
            $"/v1/workers/{id}",
            new UpdateWorkerCommand(
                id,
                "Ali",
                "Veli",
                "WRK-1",
                "Senior Electrician",
                "Maintenance",
                "+90 555 000 00 01",
                false));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        var list = await client.GetAsync("/v1/workers?activeOnly=false");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        var listBody = await list.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(
            listBody.GetProperty("data").EnumerateArray(),
            item => item.GetProperty("id").GetGuid() == id);
    }
}
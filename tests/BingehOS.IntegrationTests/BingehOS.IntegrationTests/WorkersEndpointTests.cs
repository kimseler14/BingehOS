using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Personnel.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class WorkersEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public WorkersEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/workers",
            new CreateWorkerCommand("Ali", "Veli", "EMP-1", "Maintenance", "+90 555 000 00 00", true));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/workers/{id}",
            new UpdateWorkerCommand(id, "Ali", "Veli", "+90 555 000 00 00", false));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

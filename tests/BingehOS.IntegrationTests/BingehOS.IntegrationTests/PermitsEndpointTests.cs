using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.HSE.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class PermitsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public PermitsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_Approve_ReturnsOk()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync("/v1/permits",
            new CreatePermitToWorkCommand("Hot Work", "Welding permit", null, null));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();

        var approve = await client.PatchAsJsonAsync($"/v1/permits/{id}/approve",
            new ApprovePermitToWorkCommand(id, Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.OK, approve.StatusCode);
    }
}

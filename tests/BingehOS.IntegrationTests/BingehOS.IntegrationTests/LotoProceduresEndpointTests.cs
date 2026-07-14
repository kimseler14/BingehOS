using System.Net;
using System.Net.Http.Json;
using BingehOS.Modules.HSE.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class LotoProceduresEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public LotoProceduresEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_Verify_ReturnsOk()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync("/v1/loto-procedures",
            new CreateLotoProcedureCommand("[\"Lock\", \"Tag\"]", Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();

        var verify = await client.PatchAsJsonAsync($"/v1/loto-procedures/{id}/verify",
            new VerifyLotoProcedureCommand(id, Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.OK, verify.StatusCode);
    }
}

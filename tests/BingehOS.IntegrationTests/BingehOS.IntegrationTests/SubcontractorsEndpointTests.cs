using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Personnel.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class SubcontractorsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public SubcontractorsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync("/v1/subcontractors",
            new CreateSubcontractorCommand("Acme Ltd", "TAX-123", "John Doe", "+90 555 000 00 00", true));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/subcontractors/{id}",
            new UpdateSubcontractorCommand(id, "Beta Inc", "TAX-456", "Jane Smith", "+90 555 111 11 11", true));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

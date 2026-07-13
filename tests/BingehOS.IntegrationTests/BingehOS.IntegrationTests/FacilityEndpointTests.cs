using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Facility.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class FacilityEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public FacilityEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/facilities",
            new CreateFacilityCommand("HQ", "HQ-1", "Istanbul", "Europe/Istanbul", null));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/facilities/{id}",
            new UpdateFacilityCommand(id, "HQ Renamed", "Ankara", "Europe/Istanbul", null));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

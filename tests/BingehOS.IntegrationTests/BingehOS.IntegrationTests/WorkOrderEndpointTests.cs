using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Maintenance.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class WorkOrderEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public WorkOrderEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_ChangeStatus_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/work-orders",
            new { assetId = "22222222-2222-2222-2222-222222222222", description = "Fix HVAC" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/work-orders/{id}/status",
            new ChangeWorkOrderStatusCommand(id, "Requested", false, false));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

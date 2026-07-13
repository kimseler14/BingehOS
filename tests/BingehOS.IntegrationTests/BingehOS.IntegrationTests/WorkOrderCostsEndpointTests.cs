using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Finance.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class WorkOrderCostsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public WorkOrderCostsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Approve_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/work-order-costs",
            new CreateWorkOrderCostCommand(Guid.NewGuid(), 150000, "TRY", "Pending", false));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/work-order-costs/{id}/approve", new { });
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

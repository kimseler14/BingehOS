using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FacilityOS.Infrastructure;
using FacilityOS.Modules.Maintenance.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FacilityOS.IntegrationTests;

public class WorkOrderEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public WorkOrderEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_ChangeStatus_ReturnsOk()
    {
        var app = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("ConnectionStrings:Postgres", _fx.ConnectionString));
        await using var _ = app;

        // Apply migrations so the WorkOrders table + RLS policy exist.
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        using var client = app.CreateClient();

        var tenant = "11111111-1111-1111-1111-111111111111";
        client.DefaultRequestHeaders.Add("x-tenant-id", tenant);

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

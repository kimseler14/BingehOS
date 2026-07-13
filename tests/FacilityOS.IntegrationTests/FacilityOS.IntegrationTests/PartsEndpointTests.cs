using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FacilityOS.Modules.Inventory.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FacilityOS.IntegrationTests;

public class PartsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public PartsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        var app = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("ConnectionStrings:Postgres", _fx.ConnectionString));
        await using var _ = app;

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FacilityOS.Infrastructure.AppDbContext>();
            await db.Database.MigrateAsync();
        }

        using var client = app.CreateClient();
        client.DefaultRequestHeaders.Add("x-tenant-id", "11111111-1111-1111-1111-111111111111");

        var create = await client.PostAsJsonAsync("/v1/parts",
            new CreatePartCommand("PN-1", "Bearing", "6001-2RS", "pcs", true));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/parts/{id}",
            new UpdatePartCommand(id, "Bearing V2", "6001-2RS", "pcs", false));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

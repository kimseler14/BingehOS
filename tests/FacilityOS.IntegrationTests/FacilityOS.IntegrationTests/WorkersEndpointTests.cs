using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FacilityOS.Modules.Personnel.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FacilityOS.IntegrationTests;

public class WorkersEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public WorkersEndpointTests(TestContainerFixture fx) => _fx = fx;

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

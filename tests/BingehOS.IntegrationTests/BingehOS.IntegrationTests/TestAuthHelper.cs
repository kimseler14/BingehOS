using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BingehOS.IntegrationTests;

public static class TestAuthHelper
{
    public static async Task<HttpClient> GetAuthenticatedClientAsync(TestContainerFixture fx)
    {
        var app = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("ConnectionStrings:Postgres", fx.ConnectionString));
        await using var _ = app;

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BingehOS.Infrastructure.AppDbContext>();
            await db.Database.MigrateAsync();
        }

        var client = app.CreateClient();
        // The login request is unauthenticated, so the tenant must be supplied via the
        // x-tenant-id header. The seeded SystemAdmin lives under this system tenant, and
        // the JWT claim derived from it drives tenant resolution on later requests.
        client.DefaultRequestHeaders.Add("x-tenant-id", "11111111-1111-1111-1111-111111111111");

        var login = await client.PostAsJsonAsync("/v1/auth/login", new { Email = "admin@system", Password = "admin" });
        var loginBody = await login.Content.ReadAsStringAsync();
        Assert.True(login.IsSuccessStatusCode, $"LOGIN FAILED status={login.StatusCode} body={loginBody}");

        var body = await login.Content.ReadFromJsonAsync<JsonElement>();
        var token = body.GetProperty("data").GetProperty("accessToken").GetString()!;
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}

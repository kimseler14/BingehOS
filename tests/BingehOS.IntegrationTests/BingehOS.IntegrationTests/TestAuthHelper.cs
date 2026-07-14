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
    public static async Task<AuthenticatedClient> GetAuthenticatedClientAsync(TestContainerFixture fx)
    {
        var app = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("ConnectionStrings:Postgres", fx.ConnectionString));

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BingehOS.Infrastructure.AppDbContext>();
            await db.Database.MigrateAsync();
        }

        var client = app.CreateClient();
        client.DefaultRequestHeaders.Add("x-tenant-id", "11111111-1111-1111-1111-111111111111");

        var login = await client.PostAsJsonAsync("/v1/auth/login", new { Email = "admin@system", Password = "admin" });
        var loginBody = await login.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var body = await login.Content.ReadFromJsonAsync<JsonElement>();
        var token = body.GetProperty("data").GetProperty("accessToken").GetString()!;
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return new AuthenticatedClient(client, app);
    }
}

public class AuthenticatedClient : IAsyncDisposable
{
    public HttpClient Client { get; }
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticatedClient(HttpClient client, WebApplicationFactory<Program> factory)
    {
        Client = client;
        _factory = factory;
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return _factory.DisposeAsync();
    }
}

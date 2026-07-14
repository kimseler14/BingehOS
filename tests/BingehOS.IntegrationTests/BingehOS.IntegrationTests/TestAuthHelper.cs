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

        var migrationOptions = new DbContextOptionsBuilder<BingehOS.Infrastructure.AppDbContext>()
            .UseNpgsql(fx.AdminConnectionString)
            .Options;
        await using (var db = new BingehOS.Infrastructure.AppDbContext(
                         migrationOptions,
                         new FixedTenantProvider(Guid.Empty)))
        {
            await db.Database.MigrateAsync();
        }
        await fx.GrantApplicationRoleAsync();

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

    private sealed class FixedTenantProvider(Guid tenantId) : BingehOS.Infrastructure.ITenantProvider
    {
        public Guid CurrentTenantId { get; } = tenantId;
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

    public HttpClient CreateClient(Guid tenantId)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("x-tenant-id", tenantId.ToString());
        return client;
    }

    public async Task SeedUserAsync(Guid tenantId, string email, string password, string fullName)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BingehOS.Infrastructure.AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<BingehOS.Infrastructure.Security.IPasswordHasher>();
        db.CurrentTenantId = tenantId;

        var role = BingehOS.Modules.Identity.Domain.Role.Create(tenantId, "User", "Standard user");
        var user = BingehOS.Modules.Identity.Domain.User.Create(
            tenantId,
            email,
            hasher.Hash(password),
            fullName);
        db.Roles.Add(role);
        db.Users.Add(user);
        db.UserRoles.Add(BingehOS.Modules.Identity.Domain.UserRole.Create(
            tenantId,
            user.Id,
            role.Id,
            user.Id));
        await db.SaveChangesAsync();
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return _factory.DisposeAsync();
    }
}

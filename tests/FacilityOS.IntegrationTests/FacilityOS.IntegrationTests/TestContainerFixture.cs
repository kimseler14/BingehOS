using Testcontainers.PostgreSql;
using Xunit;

namespace FacilityOS.IntegrationTests;

public class TestContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithUsername("postgres").WithPassword("postgres").WithDatabase("facilityos")
        .Build();

    public string ConnectionString => Container.GetConnectionString();

    public Task InitializeAsync() => Container.StartAsync();
    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}

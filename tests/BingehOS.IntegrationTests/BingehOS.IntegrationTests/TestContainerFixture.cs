using Testcontainers.PostgreSql;
using Xunit;
using Npgsql;

namespace BingehOS.IntegrationTests;

public class TestContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithUsername("postgres").WithPassword("postgres").WithDatabase("bingehos")
        .Build();

    public string AdminConnectionString => Container.GetConnectionString();
    public string ConnectionString => AdminConnectionString
        .Replace("Username=postgres", "Username=bingehos_app")
        .Replace("Password=postgres", "Password=bingehos_app");

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        await using var connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'bingehos_app') THEN
                    CREATE ROLE bingehos_app LOGIN PASSWORD 'bingehos_app' NOSUPERUSER NOBYPASSRLS;
                END IF;
            END
            $$;
            """;
        await command.ExecuteNonQueryAsync();
    }

    public async Task GrantApplicationRoleAsync()
    {
        await using var connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            GRANT CONNECT ON DATABASE bingehos TO bingehos_app;
            GRANT USAGE ON SCHEMA public TO bingehos_app;
            GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO bingehos_app;
            GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO bingehos_app;
            """;
        await command.ExecuteNonQueryAsync();
    }

    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}

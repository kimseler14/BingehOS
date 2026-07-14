using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Personnel.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class EmployeesEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public EmployeesEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync("/v1/employees",
            new CreateEmployeeCommand("Ali", "Veli", "EMP-1", "Maintenance", "+90 555 000 00 00", true));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/employees/{id}",
            new UpdateEmployeeCommand(id, "Ali", "Veli", "+90 555 000 00 00", false));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}

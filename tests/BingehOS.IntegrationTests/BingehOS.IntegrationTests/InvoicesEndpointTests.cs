using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Finance.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class InvoicesEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public InvoicesEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Get_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/invoices",
            new CreateInvoiceCommand("INV-001", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(30), null, 150000, "TRY", "Draft", "Purchase"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();

        var get = await client.GetAsync($"/v1/invoices/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }
}

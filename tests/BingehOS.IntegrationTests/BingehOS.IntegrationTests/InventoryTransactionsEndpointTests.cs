using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Inventory.Application;
using BingehOS.Modules.Inventory.Domain;
using Xunit;

namespace BingehOS.IntegrationTests;

public class InventoryTransactionsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public InventoryTransactionsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Receive_Issue_Return_And_List_Work()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var createPart = await client.PostAsJsonAsync(
            "/v1/parts",
            new CreatePartCommand("PART-INV-1", "Grease", "Synthetic grease", "kg", true));
        Assert.Equal(HttpStatusCode.Created, createPart.StatusCode);
        var createBody = await createPart.Content.ReadFromJsonAsync<JsonElement>();
        var partId = createBody.GetProperty("data").GetProperty("id").GetGuid();

        var receive = await client.PostAsJsonAsync(
            $"/v1/parts/{partId}/receive",
            new ReceivePartCommand(partId, 10, null, null, "Initial receipt"));
        Assert.Equal(HttpStatusCode.OK, receive.StatusCode);
        var receiveBody = await receive.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(10, receiveBody.GetProperty("data").GetProperty("currentStock").GetInt32());

        var issue = await client.PostAsJsonAsync(
            $"/v1/parts/{partId}/issue",
            new IssuePartCommand(partId, 3, null, null, "Issued to work order"));
        Assert.Equal(HttpStatusCode.OK, issue.StatusCode);
        var issueBody = await issue.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(7, issueBody.GetProperty("data").GetProperty("currentStock").GetInt32());

        var returnTx = await client.PostAsJsonAsync(
            $"/v1/parts/{partId}/return",
            new ReturnPartCommand(partId, 2, null, null, "Returned unused"));
        Assert.Equal(HttpStatusCode.OK, returnTx.StatusCode);
        var returnBody = await returnTx.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(9, returnBody.GetProperty("data").GetProperty("currentStock").GetInt32());

        var tooMuch = await client.PostAsJsonAsync(
            $"/v1/parts/{partId}/issue",
            new IssuePartCommand(partId, 10));
        Assert.Equal(HttpStatusCode.BadRequest, tooMuch.StatusCode);

        var list = await client.GetAsync($"/v1/inventory/transactions?partId={partId}&take=50");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        var listBody = await list.Content.ReadFromJsonAsync<JsonElement>();
        var items = listBody.GetProperty("data").EnumerateArray().ToList();
        Assert.Equal(3, items.Count);
        Assert.All(items, item => Assert.Equal(partId, item.GetProperty("partId").GetGuid()));
        Assert.Contains(items, item => item.GetProperty("type").GetInt32() == (int)TransactionType.Receiving);
        Assert.Contains(items, item => item.GetProperty("type").GetInt32() == (int)TransactionType.Issue);
        Assert.Contains(items, item => item.GetProperty("type").GetInt32() == (int)TransactionType.Return);
    }
}

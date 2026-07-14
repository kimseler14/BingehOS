using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Automation.Application;
using BingehOS.Modules.Automation.Domain;
using Xunit;

namespace BingehOS.IntegrationTests;

public class AutomationRulesEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;

    public AutomationRulesEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Crud_And_WorkOrderCreated_Trigger_Are_Supported()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync(
            "/v1/automation-rules",
            new CreateAutomationRuleCommand(
                "İş emri bildirimi",
                "Yeni iş emri oluştuğunda kayıt oluştur.",
                true,
                AutomationTriggerType.WorkOrderCreated,
                """{"field":"status","operator":"equals","value":"Draft"}""",
                AutomationActionType.SendNotification,
                """{"message":"Yeni iş emri oluşturuldu."}"""));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var createBody = await create.Content.ReadFromJsonAsync<JsonElement>();
        var ruleId = createBody.GetProperty("data").GetProperty("id").GetGuid();

        var listed = await client.GetAsync("/v1/automation-rules?skip=0&take=20");
        Assert.Equal(HttpStatusCode.OK, listed.StatusCode);

        var workOrder = await client.PostAsJsonAsync(
            "/v1/work-orders",
            new { assetId = "22222222-2222-2222-2222-222222222222", description = "Otomasyon testi" });
        Assert.Equal(HttpStatusCode.Created, workOrder.StatusCode);

        var executions = await client.GetAsync($"/v1/automation-rules/{ruleId}/executions");
        Assert.Equal(HttpStatusCode.OK, executions.StatusCode);
        var executionBody = await executions.Content.ReadFromJsonAsync<JsonElement>();
        var executionData = executionBody.GetProperty("data");
        Assert.True(executionData.GetArrayLength() >= 1);
        Assert.True(executionData[0].GetProperty("success").GetBoolean());
    }
}

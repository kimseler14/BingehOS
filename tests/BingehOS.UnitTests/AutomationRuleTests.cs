using BingehOS.Modules.Automation.Domain;

namespace BingehOS.UnitTests;

public class AutomationRuleTests
{
    [Fact]
    public void Create_And_Update_Preserve_Automation_Configuration()
    {
        var tenantId = Guid.NewGuid();
        var rule = AutomationRule.Create(
            tenantId,
            "Düşük stok bildirimi",
            "Parça stoğu kritik seviyeye indiğinde bildir.",
            true,
            AutomationTriggerType.InventoryStockLow,
            """{"field":"currentStock","operator":"less","value":5}""",
            AutomationActionType.SendNotification,
            """{"message":"Stok kritik."}""");

        rule.Update(
            "Güncellenmiş kural",
            null,
            false,
            AutomationTriggerType.WorkOrderCreated,
            "{}",
            AutomationActionType.CreateWorkOrder,
            """{"description":"Takip iş emri"}""");

        Assert.Equal(tenantId, rule.TenantId);
        Assert.Equal("Güncellenmiş kural", rule.Name);
        Assert.False(rule.IsEnabled);
        Assert.Equal(AutomationTriggerType.WorkOrderCreated, rule.TriggerType);
        Assert.Equal(AutomationActionType.CreateWorkOrder, rule.ActionType);
    }

    [Fact]
    public void SoftDelete_Disables_The_Rule()
    {
        var rule = AutomationRule.Create(
            Guid.NewGuid(),
            "Rule",
            null,
            true,
            AutomationTriggerType.WorkOrderCreated,
            "{}",
            AutomationActionType.SendNotification,
            "{}");

        rule.SoftDelete();

        Assert.True(rule.IsDeleted);
        Assert.False(rule.IsEnabled);
    }
}

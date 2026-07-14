using BingehOS.Shared;

namespace BingehOS.Modules.Automation.Domain;

public class AutomationRuleExecution : BaseEntity
{
    public Guid RuleId { get; private set; }
    public DateTimeOffset ExecutedAt { get; private set; }
    public bool Success { get; private set; }
    public string Detail { get; private set; } = string.Empty;

    public static AutomationRuleExecution Create(
        Guid tenantId,
        Guid ruleId,
        bool success,
        string detail)
        => new()
        {
            TenantId = tenantId,
            RuleId = ruleId,
            ExecutedAt = DateTimeOffset.UtcNow,
            Success = success,
            Detail = detail
        };
}

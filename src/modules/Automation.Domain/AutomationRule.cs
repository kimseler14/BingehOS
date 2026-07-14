using BingehOS.Shared;

namespace BingehOS.Modules.Automation.Domain;

public class AutomationRule : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsEnabled { get; private set; } = true;
    public AutomationTriggerType TriggerType { get; private set; }
    public string ConditionJson { get; private set; } = "{}";
    public AutomationActionType ActionType { get; private set; }
    public string ActionParametersJson { get; private set; } = "{}";

    public static AutomationRule Create(
        Guid tenantId,
        string name,
        string? description,
        bool isEnabled,
        AutomationTriggerType triggerType,
        string conditionJson,
        AutomationActionType actionType,
        string actionParametersJson)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            Description = description,
            IsEnabled = isEnabled,
            TriggerType = triggerType,
            ConditionJson = conditionJson,
            ActionType = actionType,
            ActionParametersJson = actionParametersJson
        };

    public void Update(
        string name,
        string? description,
        bool isEnabled,
        AutomationTriggerType triggerType,
        string conditionJson,
        AutomationActionType actionType,
        string actionParametersJson)
    {
        Name = name;
        Description = description;
        IsEnabled = isEnabled;
        TriggerType = triggerType;
        ConditionJson = conditionJson;
        ActionType = actionType;
        ActionParametersJson = actionParametersJson;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        IsEnabled = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

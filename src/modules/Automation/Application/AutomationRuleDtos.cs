using BingehOS.Modules.Automation.Domain;
using MediatR;

namespace BingehOS.Modules.Automation.Application;

public record AutomationRuleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsEnabled,
    AutomationTriggerType TriggerType,
    string ConditionJson,
    AutomationActionType ActionType,
    string ActionParametersJson);

public record AutomationRuleExecutionDto(
    Guid Id,
    Guid RuleId,
    DateTimeOffset ExecutedAt,
    bool Success,
    string Detail);

public record CreateAutomationRuleCommand(
    string Name,
    string? Description,
    bool IsEnabled,
    AutomationTriggerType TriggerType,
    string ConditionJson,
    AutomationActionType ActionType,
    string ActionParametersJson) : IRequest<Guid>;

public record UpdateAutomationRuleCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsEnabled,
    AutomationTriggerType TriggerType,
    string ConditionJson,
    AutomationActionType ActionType,
    string ActionParametersJson) : IRequest<AutomationRuleDto?>;

public record GetAutomationRulesQuery(int Skip = 0, int Take = 50) : IRequest<IReadOnlyList<AutomationRuleDto>>;

public record GetAutomationRuleQuery(Guid Id) : IRequest<AutomationRuleDto?>;

public record DeleteAutomationRuleCommand(Guid Id) : IRequest<bool>;

public record GetAutomationRuleExecutionsQuery(Guid RuleId, int Skip = 0, int Take = 50)
    : IRequest<IReadOnlyList<AutomationRuleExecutionDto>>;

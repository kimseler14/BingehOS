using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Automation.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Automation.Application;

public sealed class CreateAutomationRuleHandler(AppDbContext db) : IRequestHandler<CreateAutomationRuleCommand, Guid>
{
    public async Task<Guid> Handle(CreateAutomationRuleCommand command, CancellationToken cancellationToken)
    {
        var rule = AutomationRule.Create(
            db.CurrentTenantId,
            command.Name,
            command.Description,
            command.IsEnabled,
            command.TriggerType,
            command.ConditionJson,
            command.ActionType,
            command.ActionParametersJson);

        db.AutomationRules.Add(rule);
        await db.SaveChangesAsync(cancellationToken);
        return rule.Id;
    }
}

public sealed class UpdateAutomationRuleHandler(AppDbContext db)
    : IRequestHandler<UpdateAutomationRuleCommand, AutomationRuleDto?>
{
    public async Task<AutomationRuleDto?> Handle(
        UpdateAutomationRuleCommand command,
        CancellationToken cancellationToken)
    {
        var rule = await db.AutomationRules
            .SingleOrDefaultAsync(e => e.Id == command.Id && e.TenantId == db.CurrentTenantId, cancellationToken);
        if (rule is null)
            return null;

        rule.Update(
            command.Name,
            command.Description,
            command.IsEnabled,
            command.TriggerType,
            command.ConditionJson,
            command.ActionType,
            command.ActionParametersJson);
        await db.SaveChangesAsync(cancellationToken);
        return rule.ToDto();
    }
}

public sealed class GetAutomationRulesHandler(AppDbContext db)
    : IRequestHandler<GetAutomationRulesQuery, IReadOnlyList<AutomationRuleDto>>
{
    public async Task<IReadOnlyList<AutomationRuleDto>> Handle(
        GetAutomationRulesQuery query,
        CancellationToken cancellationToken)
        => await db.AutomationRules
            .Where(e => e.TenantId == db.CurrentTenantId)
            .OrderByDescending(e => e.CreatedAt)
            .ApplyPaging(query.Skip, query.Take)
            .Select(e => new AutomationRuleDto(
                e.Id,
                e.Name,
                e.Description,
                e.IsEnabled,
                e.TriggerType,
                e.ConditionJson,
                e.ActionType,
                e.ActionParametersJson))
            .ToListAsync(cancellationToken);
}

public sealed class GetAutomationRuleHandler(AppDbContext db)
    : IRequestHandler<GetAutomationRuleQuery, AutomationRuleDto?>
{
    public async Task<AutomationRuleDto?> Handle(
        GetAutomationRuleQuery query,
        CancellationToken cancellationToken)
        => await db.AutomationRules
            .Where(e => e.Id == query.Id && e.TenantId == db.CurrentTenantId)
            .Select(e => new AutomationRuleDto(
                e.Id,
                e.Name,
                e.Description,
                e.IsEnabled,
                e.TriggerType,
                e.ConditionJson,
                e.ActionType,
                e.ActionParametersJson))
            .SingleOrDefaultAsync(cancellationToken);
}

public sealed class DeleteAutomationRuleHandler(AppDbContext db) : IRequestHandler<DeleteAutomationRuleCommand, bool>
{
    public async Task<bool> Handle(DeleteAutomationRuleCommand command, CancellationToken cancellationToken)
    {
        var rule = await db.AutomationRules
            .SingleOrDefaultAsync(e => e.Id == command.Id && e.TenantId == db.CurrentTenantId, cancellationToken);
        if (rule is null)
            return false;

        rule.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class GetAutomationRuleExecutionsHandler(AppDbContext db)
    : IRequestHandler<GetAutomationRuleExecutionsQuery, IReadOnlyList<AutomationRuleExecutionDto>>
{
    public async Task<IReadOnlyList<AutomationRuleExecutionDto>> Handle(
        GetAutomationRuleExecutionsQuery query,
        CancellationToken cancellationToken)
        => await db.AutomationRuleExecutions
            .Where(e => e.RuleId == query.RuleId && e.TenantId == db.CurrentTenantId)
            .OrderByDescending(e => e.ExecutedAt)
            .ApplyPaging(query.Skip, query.Take)
            .Select(e => new AutomationRuleExecutionDto(
                e.Id,
                e.RuleId,
                e.ExecutedAt,
                e.Success,
                e.Detail))
            .ToListAsync(cancellationToken);
}

internal static class AutomationRuleMappings
{
    public static AutomationRuleDto ToDto(this AutomationRule rule)
        => new(
            rule.Id,
            rule.Name,
            rule.Description,
            rule.IsEnabled,
            rule.TriggerType,
            rule.ConditionJson,
            rule.ActionType,
            rule.ActionParametersJson);
}

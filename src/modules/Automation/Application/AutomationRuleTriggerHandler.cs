using System.Globalization;
using System.Text.Json;
using BingehOS.Infrastructure;
using BingehOS.Modules.Automation.Domain;
using BingehOS.Modules.Maintenance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Automation.Application;

public sealed class AutomationRuleTriggerHandler(AppDbContext db)
    : INotificationHandler<AutomationTriggerNotification>
{
    public async Task Handle(
        AutomationTriggerNotification notification,
        CancellationToken cancellationToken)
    {
        var rules = await db.AutomationRules
            .Where(e =>
                e.TenantId == notification.TenantId &&
                !e.IsDeleted &&
                e.IsEnabled &&
                e.TriggerType == notification.TriggerType)
            .ToListAsync(cancellationToken);

        foreach (var rule in rules)
        {
            if (!AutomationConditionEvaluator.Evaluate(rule.ConditionJson, notification.Payload))
                continue;

            var success = true;
            string detail;
            try
            {
                detail = ExecuteAction(rule, notification);
            }
            catch (Exception exception)
            {
                success = false;
                detail = exception.Message;
            }

            db.AutomationRuleExecutions.Add(
                AutomationRuleExecution.Create(notification.TenantId, rule.Id, success, detail));
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private string ExecuteAction(
        AutomationRule rule,
        AutomationTriggerNotification notification)
    {
        using var parameters = ParseParameters(rule.ActionParametersJson);

        return rule.ActionType switch
        {
            AutomationActionType.SendNotification =>
                ExecuteNotification(parameters.RootElement, rule.Name),
            AutomationActionType.CreateWorkOrder =>
                ExecuteCreateWorkOrder(parameters.RootElement, notification),
            AutomationActionType.AdjustPriority =>
                ExecuteAdjustPriority(parameters.RootElement, notification),
            _ => throw new NotSupportedException($"Unsupported automation action: {rule.ActionType}.")
        };
    }

    private static string ExecuteNotification(JsonElement parameters, string ruleName)
    {
        var message = parameters.TryGetProperty("message", out var messageElement)
            ? messageElement.GetString()
            : null;
        return $"Notification queued for rule '{ruleName}': {message ?? "Automation rule triggered."}";
    }

    private string ExecuteCreateWorkOrder(
        JsonElement parameters,
        AutomationTriggerNotification notification)
    {
        var assetId = GetGuid(parameters, "assetId") ??
                      GetGuid(notification.Payload, "assetId");
        if (assetId is null)
            throw new InvalidOperationException("CreateWorkOrder requires an assetId.");

        var description = parameters.TryGetProperty("description", out var descriptionElement)
            ? descriptionElement.GetString()
            : $"Otomasyon kuralı tetiklendi: {notification.TriggerType}";
        var workOrder = WorkOrder.Create(
            notification.TenantId,
            assetId.Value,
            description ?? "Automation rule triggered.");
        db.WorkOrders.Add(workOrder);
        return $"Work order {workOrder.Id} created.";
    }

    private string ExecuteAdjustPriority(
        JsonElement parameters,
        AutomationTriggerNotification notification)
    {
        var workOrderId = GetGuid(parameters, "workOrderId") ??
                          GetGuid(notification.Payload, "workOrderId");
        if (workOrderId is null)
            throw new InvalidOperationException("AdjustPriority requires a workOrderId.");

        if (!parameters.TryGetProperty("priority", out var priorityElement) ||
            !priorityElement.TryGetInt32(out var priority))
        {
            throw new InvalidOperationException("AdjustPriority requires an integer priority.");
        }

        var workOrder = db.WorkOrders.SingleOrDefault(e =>
            e.Id == workOrderId.Value && e.TenantId == notification.TenantId);
        if (workOrder is null)
            throw new InvalidOperationException($"Work order {workOrderId} was not found.");

        workOrder.SetPriority(priority);
        return $"Work order {workOrder.Id} priority changed to {priority}.";
    }

    private static Guid? GetGuid(JsonElement parameters, string name)
        => parameters.TryGetProperty(name, out var value) &&
           Guid.TryParse(value.GetString(), out var parsed)
            ? parsed
            : null;

    private static Guid? GetGuid(
        IReadOnlyDictionary<string, object?> payload,
        string name)
        => payload.TryGetValue(name, out var value) && Guid.TryParse(
            Convert.ToString(value, CultureInfo.InvariantCulture),
            out var parsed)
            ? parsed
            : null;

    private static JsonDocument ParseParameters(string parametersJson)
        => JsonDocument.Parse(string.IsNullOrWhiteSpace(parametersJson) ? "{}" : parametersJson);
}

using BingehOS.Modules.Automation.Domain;
using MediatR;

namespace BingehOS.Modules.Automation.Application;

public sealed record AutomationTriggerNotification(
    Guid TenantId,
    AutomationTriggerType TriggerType,
    IReadOnlyDictionary<string, object?> Payload) : INotification;

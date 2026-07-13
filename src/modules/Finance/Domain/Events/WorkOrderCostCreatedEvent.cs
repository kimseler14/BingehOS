using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public record WorkOrderCostCreatedEvent(Guid WorkOrderCostId, Guid WorkOrderId, decimal AmountMinor) : IEvent;

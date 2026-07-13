using BingehOS.Shared;

namespace BingehOS.Modules.Maintenance.Domain;

public record WorkOrderCreatedEvent(Guid WorkOrderId, Guid AssetId) : IEvent;

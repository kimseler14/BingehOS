using BingehOS.Shared;

namespace BingehOS.Modules.Inventory.Domain;

public record PartCreatedEvent(Guid PartId, string PartNumber, string Name) : IEvent;

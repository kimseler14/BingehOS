using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public record AssetCreatedEvent(Guid AssetId, string Name, string? SerialNumber) : IEvent;

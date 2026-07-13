using BingehOS.Shared;

namespace BingehOS.Modules.HSE.Domain;

public record PermitToWorkCreatedEvent(Guid PermitToWorkId, string Title, string Status) : IEvent;

using BingehOS.Shared;

namespace BingehOS.Modules.HSE.Domain;

public record IncidentCreatedEvent(Guid IncidentId, string Title, string Severity) : IEvent;

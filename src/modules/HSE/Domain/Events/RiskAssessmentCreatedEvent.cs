using BingehOS.Shared;

namespace BingehOS.Modules.HSE.Domain;

public record RiskAssessmentCreatedEvent(Guid RiskAssessmentId, string Title, string Level) : IEvent;

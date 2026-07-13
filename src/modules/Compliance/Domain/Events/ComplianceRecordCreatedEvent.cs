using BingehOS.Shared;

namespace BingehOS.Modules.Compliance.Domain;

public record ComplianceRecordCreatedEvent(Guid ComplianceRecordId, string Title) : IEvent;

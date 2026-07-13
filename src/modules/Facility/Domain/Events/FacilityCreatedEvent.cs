using BingehOS.Shared;

namespace BingehOS.Modules.Facility.Domain;

public record FacilityCreatedEvent(Guid FacilityId, string Name, string Code) : IEvent;

using BingehOS.Shared;

namespace BingehOS.Modules.Personnel.Domain;

public record WorkerCreatedEvent(Guid WorkerId, string FirstName, string LastName) : IEvent;

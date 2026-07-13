using BingehOS.Shared;

namespace BingehOS.Modules.Personnel.Domain;

public record EmployeeCreatedEvent(Guid EmployeeId, string FirstName, string LastName) : IEvent;

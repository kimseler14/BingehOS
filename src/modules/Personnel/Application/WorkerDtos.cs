using MediatR;

namespace FacilityOS.Modules.Personnel.Application;

public record WorkerDto(Guid Id, string FirstName, string LastName, string? EmployeeNumber, string? Department, string? Phone, bool IsActive);

public record CreateWorkerCommand(string FirstName, string LastName, string? EmployeeNumber, string? Department, string? Phone, bool IsActive) : IRequest<Guid>;

public record UpdateWorkerCommand(Guid Id, string FirstName, string LastName, string? Phone, bool IsActive) : IRequest<WorkerDto>;

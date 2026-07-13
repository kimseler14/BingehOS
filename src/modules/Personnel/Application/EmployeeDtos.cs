using MediatR;

namespace BingehOS.Modules.Personnel.Application;

public record EmployeeDto(Guid Id, string FirstName, string LastName, string? EmployeeNumber, string? Department, string? Phone, bool IsActive);

public record CreateEmployeeCommand(string FirstName, string LastName, string? EmployeeNumber, string? Department, string? Phone, bool IsActive) : IRequest<Guid>;

public record UpdateEmployeeCommand(Guid Id, string FirstName, string LastName, string? Phone, bool IsActive) : IRequest<EmployeeDto>;

public record CreateSgkRecordCommand(Guid EmployeeId, string SgkNumber, string ProfessionCode, string NaceCode, DateTime RegistrationDate) : IRequest<Guid>;

public record CreateSubcontractorCommand(string CompanyName, string TaxNumber, string? ContactPerson, string? Phone, bool IsActive) : IRequest<Guid>;

public record UpdateSubcontractorCommand(Guid Id, string CompanyName, string TaxNumber, string? ContactPerson, string? Phone, bool IsActive) : IRequest<SubcontractorDto>;

public record SubcontractorDto(Guid Id, string CompanyName, string TaxNumber, string? ContactPerson, string? Phone, bool IsActive);

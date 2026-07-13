using MediatR;

namespace BingehOS.Modules.Compliance.Application;

public record ComplianceRecordDto(Guid Id, string Title, string Description, string Status, DateTime DueDate);

public record CreateComplianceRecordCommand(string Title, string Description, string Status, DateTime DueDate) : IRequest<Guid>;

public record UpdateComplianceRecordCommand(Guid Id, string Title, string Description, string Status, DateTime DueDate) : IRequest<ComplianceRecordDto>;

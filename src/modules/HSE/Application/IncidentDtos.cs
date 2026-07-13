using MediatR;

namespace BingehOS.Modules.HSE.Application;

public record IncidentDto(Guid Id, string Title, string Description, string Severity, DateTime OccurredAt, bool IsResolved);

public record CreateIncidentCommand(string Title, string Description, string Severity, DateTime OccurredAt, bool IsResolved) : IRequest<Guid>;

public record UpdateIncidentCommand(Guid Id, string Title, string Description, string Severity, bool IsResolved) : IRequest<IncidentDto>;

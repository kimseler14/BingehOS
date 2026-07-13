using MediatR;

namespace BingehOS.Modules.HSE.Application;

public record RiskAssessmentDto(Guid Id, string Title, string Description, string Level, Guid PermitToWorkId);
public record CreateRiskAssessmentCommand(string Title, string Description, string Level, Guid PermitToWorkId) : IRequest<Guid>;

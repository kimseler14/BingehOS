using MediatR;

namespace BingehOS.Modules.HSE.Application;

public record LotoProcedureDto(Guid Id, string Steps, bool IsVerified, Guid? VerifiedBy, DateTimeOffset? VerifiedAt, Guid PermitToWorkId);
public record CreateLotoProcedureCommand(string Steps, Guid PermitToWorkId) : IRequest<Guid>;
public record VerifyLotoProcedureCommand(Guid Id, Guid VerifiedBy) : IRequest<LotoProcedureDto>;

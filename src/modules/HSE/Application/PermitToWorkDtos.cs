using MediatR;

namespace BingehOS.Modules.HSE.Application;

public record PermitToWorkDto(Guid Id, string Title, string Description, string Status, Guid? FacilityId, Guid? WorkOrderId);
public record CreatePermitToWorkCommand(string Title, string Description, Guid? FacilityId, Guid? WorkOrderId) : IRequest<Guid>;
public record ApprovePermitToWorkCommand(Guid Id, Guid ApproverId) : IRequest<PermitToWorkDto>;
public record RejectPermitToWorkCommand(Guid Id, Guid ApproverId, string RejectionReason) : IRequest<PermitToWorkDto>;

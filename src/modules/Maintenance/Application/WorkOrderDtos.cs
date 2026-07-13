using MediatR;

namespace FacilityOS.Modules.Maintenance.Application;

public record WorkOrderDto(Guid Id, Guid AssetId, string Description, string Status);

public record CreateWorkOrderCommand(Guid AssetId, string Description) : IRequest<Guid>;
public record ChangeWorkOrderStatusCommand(Guid Id, string NewStatus, bool PermitApproved, bool ESignatureCaptured) : IRequest<WorkOrderDto>;

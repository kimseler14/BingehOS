using MediatR;

namespace BingehOS.Modules.Finance.Application;

public record WorkOrderCostDto(Guid Id, Guid WorkOrderId, long AmountMinor, string Currency, string Status, bool IsApproved);

public record CreateWorkOrderCostCommand(Guid WorkOrderId, long AmountMinor, string Currency, string Status, bool IsApproved) : IRequest<Guid>;

public record ApproveWorkOrderCostCommand(Guid Id) : IRequest<WorkOrderCostDto>;

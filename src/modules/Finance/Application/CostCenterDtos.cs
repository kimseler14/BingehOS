using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public record CostCenterDto(Guid Id, string Code, string Name, Guid? ParentCostCenterId, long BudgetMinor, string Currency, bool IsActive);

public record CreateCostCenterCommand(string Code, string Name, Guid? ParentCostCenterId, long BudgetMinor, string Currency, bool IsActive) : IRequest<Guid>;

public record UpdateCostCenterCommand(Guid Id, string Code, string Name, Guid? ParentCostCenterId, long BudgetMinor, string Currency, bool IsActive) : IRequest<CostCenterDto>;

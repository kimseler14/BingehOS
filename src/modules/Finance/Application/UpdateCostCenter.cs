using BingehOS.Infrastructure;
using BingehOS.Modules.Finance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Finance.Application;

public class UpdateCostCenterHandler : IRequestHandler<UpdateCostCenterCommand, CostCenterDto>
{
    private readonly AppDbContext _db;
    public UpdateCostCenterHandler(AppDbContext db) => _db = db;

    public async Task<CostCenterDto> Handle(UpdateCostCenterCommand cmd, CancellationToken ct)
    {
        var costCenter = await _db.Set<Domain.CostCenter>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                         ?? throw new KeyNotFoundException($"CostCenter {cmd.Id} not found.");

        return new CostCenterDto(costCenter.Id, costCenter.Code, costCenter.Name, costCenter.ParentCostCenterId, costCenter.BudgetMinor, costCenter.Currency, costCenter.IsActive);
    }
}

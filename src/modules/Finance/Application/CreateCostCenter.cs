using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public class CreateCostCenterHandler : IRequestHandler<CreateCostCenterCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateCostCenterHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateCostCenterCommand cmd, CancellationToken ct)
    {
        var costCenter = Domain.CostCenter.Create(_db.CurrentTenantId, cmd.Code, cmd.Name, cmd.ParentCostCenterId, cmd.BudgetMinor, cmd.Currency, cmd.IsActive);
        _db.Set<Domain.CostCenter>().Add(costCenter);
        await _db.SaveChangesAsync(ct);
        return costCenter.Id;
    }
}

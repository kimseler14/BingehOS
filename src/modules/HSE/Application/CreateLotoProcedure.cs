using BingehOS.Infrastructure;
using BingehOS.Modules.HSE.Domain;
using MediatR;

namespace BingehOS.Modules.HSE.Application;

public class CreateLotoProcedureHandler : IRequestHandler<CreateLotoProcedureCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateLotoProcedureHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateLotoProcedureCommand cmd, CancellationToken ct)
    {
        var loto = LotoProcedure.Create(_db.CurrentTenantId, cmd.Steps, cmd.PermitToWorkId);
        _db.Set<LotoProcedure>().Add(loto);
        await _db.SaveChangesAsync(ct);
        return loto.Id;
    }
}

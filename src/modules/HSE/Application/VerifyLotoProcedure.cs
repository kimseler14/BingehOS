using BingehOS.Infrastructure;
using BingehOS.Modules.HSE.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.HSE.Application;

public class VerifyLotoProcedureHandler : IRequestHandler<VerifyLotoProcedureCommand, LotoProcedureDto>
{
    private readonly AppDbContext _db;
    public VerifyLotoProcedureHandler(AppDbContext db) => _db = db;

    public async Task<LotoProcedureDto> Handle(VerifyLotoProcedureCommand cmd, CancellationToken ct)
    {
        var loto = await _db.Set<LotoProcedure>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                   ?? throw new KeyNotFoundException($"LotoProcedure {cmd.Id} not found.");

        loto.Verify(cmd.VerifiedBy);
        await _db.SaveChangesAsync(ct);
        return new LotoProcedureDto(loto.Id, loto.Steps, loto.IsVerified, loto.VerifiedBy, loto.VerifiedAt, loto.PermitToWorkId);
    }
}

using BingehOS.Infrastructure;
using BingehOS.Modules.HSE.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.HSE.Application;

public class ApprovePermitToWorkHandler : IRequestHandler<ApprovePermitToWorkCommand, PermitToWorkDto>
{
    private readonly AppDbContext _db;
    public ApprovePermitToWorkHandler(AppDbContext db) => _db = db;

    public async Task<PermitToWorkDto> Handle(ApprovePermitToWorkCommand cmd, CancellationToken ct)
    {
        var permit = await _db.Set<PermitToWork>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                     ?? throw new KeyNotFoundException($"PermitToWork {cmd.Id} not found.");

        permit.Approve(cmd.ApproverId);
        await _db.SaveChangesAsync(ct);
        return new PermitToWorkDto(permit.Id, permit.Title, permit.Description, permit.Status, permit.FacilityId, permit.WorkOrderId);
    }
}

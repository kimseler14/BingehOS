using BingehOS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Inventory.Application;

public class UpdatePartHandler : IRequestHandler<UpdatePartCommand, PartDto>
{
    private readonly AppDbContext _db;
    public UpdatePartHandler(AppDbContext db) => _db = db;

    public async Task<PartDto> Handle(UpdatePartCommand cmd, CancellationToken ct)
    {
        var part = await _db.Set<Domain.Part>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                    ?? throw new KeyNotFoundException($"Part {cmd.Id} not found.");

        part.Rename(cmd.Name);
        if (!cmd.IsActive) part.Deactivate();

        await _db.SaveChangesAsync(ct);
        return new PartDto(part.Id, part.PartNumber, part.Name, part.Description, part.UnitOfMeasure, part.IsActive);
    }
}

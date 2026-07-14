using BingehOS.Infrastructure;
using BingehOS.Modules.Personnel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Personnel.Application;

public class UpdateWorkerHandler : IRequestHandler<UpdateWorkerCommand, WorkerDto>
{
    private readonly AppDbContext _db;

    public UpdateWorkerHandler(AppDbContext db) => _db = db;

    public async Task<WorkerDto> Handle(UpdateWorkerCommand cmd, CancellationToken ct)
    {
        var worker = await _db.Set<Worker>()
                          .FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                      ?? throw new KeyNotFoundException($"Worker {cmd.Id} not found.");

        worker.Update(
            cmd.FirstName,
            cmd.LastName,
            cmd.EmployeeNumber,
            cmd.Trade,
            cmd.Department,
            cmd.Phone);
        if (cmd.IsActive) worker.Activate(); else worker.Deactivate();

        await _db.SaveChangesAsync(ct);
        return new WorkerDto(
            worker.Id,
            worker.FirstName,
            worker.LastName,
            worker.EmployeeNumber,
            worker.Trade,
            worker.Department,
            worker.Phone,
            worker.IsActive);
    }
}
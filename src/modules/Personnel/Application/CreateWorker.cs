using FacilityOS.Infrastructure;
using MediatR;

namespace FacilityOS.Modules.Personnel.Application;

public class CreateWorkerHandler : IRequestHandler<CreateWorkerCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateWorkerHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateWorkerCommand cmd, CancellationToken ct)
    {
        var worker = Domain.Worker.Create(_db.CurrentTenantId, cmd.FirstName, cmd.LastName, cmd.EmployeeNumber, cmd.Department, cmd.Phone, cmd.IsActive);
        _db.Set<Domain.Worker>().Add(worker);
        await _db.SaveChangesAsync(ct);
        return worker.Id;
    }
}

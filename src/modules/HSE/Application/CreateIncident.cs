using BingehOS.Infrastructure;
using MediatR;

namespace BingehOS.Modules.HSE.Application;

public class CreateIncidentHandler : IRequestHandler<CreateIncidentCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateIncidentHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateIncidentCommand cmd, CancellationToken ct)
    {
        var incident = Domain.Incident.Create(_db.CurrentTenantId, cmd.Title, cmd.Description, cmd.Severity, cmd.OccurredAt, cmd.IsResolved);
        _db.Set<Domain.Incident>().Add(incident);
        await _db.SaveChangesAsync(ct);
        return incident.Id;
    }
}

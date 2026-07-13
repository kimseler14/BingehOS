using FacilityOS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Modules.HSE.Application;

public class UpdateIncidentHandler : IRequestHandler<UpdateIncidentCommand, IncidentDto>
{
    private readonly AppDbContext _db;
    public UpdateIncidentHandler(AppDbContext db) => _db = db;

    public async Task<IncidentDto> Handle(UpdateIncidentCommand cmd, CancellationToken ct)
    {
        var incident = await _db.Set<Domain.Incident>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                        ?? throw new KeyNotFoundException($"Incident {cmd.Id} not found.");

        incident.Rename(cmd.Title);
        incident.ChangeDescription(cmd.Description);
        incident.SetSeverity(cmd.Severity);
        if (cmd.IsResolved && !incident.IsResolved) incident.Resolve();

        await _db.SaveChangesAsync(ct);
        return new IncidentDto(incident.Id, incident.Title, incident.Description, incident.Severity, incident.OccurredAt, incident.IsResolved);
    }
}

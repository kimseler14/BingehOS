using BingehOS.Infrastructure;
using BingehOS.Modules.Maintenance.Domain;
using MediatR;

namespace BingehOS.Modules.Maintenance.Application;

public class CreateJobPlanTemplateHandler : IRequestHandler<CreateJobPlanTemplateCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateJobPlanTemplateHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateJobPlanTemplateCommand cmd, CancellationToken ct)
    {
        var template = JobPlanTemplate.Create(_db.CurrentTenantId, cmd.Name, cmd.Description, cmd.AssetType, cmd.Steps, cmd.EstimatedDurationMinutes, cmd.RequiredPpe, cmd.RequiredPermitType, cmd.RecommendedParts);
        _db.Set<JobPlanTemplate>().Add(template);
        await _db.SaveChangesAsync(ct);
        return template.Id;
    }
}

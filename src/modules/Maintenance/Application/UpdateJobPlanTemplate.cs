using BingehOS.Infrastructure;
using BingehOS.Modules.Maintenance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Maintenance.Application;

public class UpdateJobPlanTemplateHandler : IRequestHandler<UpdateJobPlanTemplateCommand, JobPlanTemplateDto>
{
    private readonly AppDbContext _db;
    public UpdateJobPlanTemplateHandler(AppDbContext db) => _db = db;

    public async Task<JobPlanTemplateDto> Handle(UpdateJobPlanTemplateCommand cmd, CancellationToken ct)
    {
        var template = await _db.Set<JobPlanTemplate>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                     ?? throw new KeyNotFoundException($"JobPlanTemplate {cmd.Id} not found.");

        template.Update(cmd.Name, cmd.Description, cmd.AssetType, cmd.Steps, cmd.EstimatedDurationMinutes, cmd.RequiredPpe, cmd.RequiredPermitType, cmd.RecommendedParts);
        await _db.SaveChangesAsync(ct);
        return new JobPlanTemplateDto(template.Id, template.Name, template.Description, template.AssetType, template.Steps, template.EstimatedDurationMinutes, template.RequiredPpe, template.RequiredPermitType, template.RecommendedParts);
    }
}

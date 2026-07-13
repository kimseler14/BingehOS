using BingehOS.Modules.Maintenance.Domain;
using MediatR;

namespace BingehOS.Modules.Maintenance.Application;

public record JobPlanTemplateDto(Guid Id, string Name, string Description, string AssetType, string Steps, int? EstimatedDurationMinutes, string RequiredPpe, string RequiredPermitType, string RecommendedParts);

public record CreateJobPlanTemplateCommand(string Name, string Description, string AssetType, string Steps, int? EstimatedDurationMinutes, string RequiredPpe, string RequiredPermitType, string RecommendedParts) : IRequest<Guid>;

public record UpdateJobPlanTemplateCommand(Guid Id, string Name, string Description, string AssetType, string Steps, int? EstimatedDurationMinutes, string RequiredPpe, string RequiredPermitType, string RecommendedParts) : IRequest<JobPlanTemplateDto>;

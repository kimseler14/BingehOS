using FacilityOS.Modules.Asset.Domain;
using MediatR;

namespace FacilityOS.Modules.Asset.Application;

public record AssetDto(Guid Id, string Name, string? SerialNumber, string? LocationCode, string Criticality);

public record CreateAssetCommand(string Name, string? SerialNumber, string? LocationCode, AssetCriticality Criticality) : IRequest<Guid>;

public record UpdateAssetCommand(Guid Id, string Name, string? LocationCode, AssetCriticality Criticality) : IRequest<AssetDto>;

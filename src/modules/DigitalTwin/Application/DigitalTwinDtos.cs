using BingehOS.Modules.DigitalTwin.Domain;
using MediatR;

namespace BingehOS.Modules.DigitalTwin.Application;

public sealed record FloorPlanDto(
    Guid Id,
    string Name,
    Guid? FacilityId,
    string? ImageUrl,
    int Width,
    int Height);

public sealed record AssetPositionDto(
    Guid Id,
    Guid AssetId,
    Guid FloorPlanId,
    double X,
    double Y,
    string AssetName,
    string Criticality);

public sealed record CreateFloorPlanCommand(
    string Name,
    Guid? FacilityId,
    string? ImageUrl,
    int Width,
    int Height) : IRequest<Guid>;

public sealed record UpdateFloorPlanCommand(
    Guid Id,
    string Name,
    Guid? FacilityId,
    string? ImageUrl,
    int Width,
    int Height) : IRequest<FloorPlanDto?>;

public sealed record GetFloorPlansQuery(int Skip = 0, int Take = 50)
    : IRequest<IReadOnlyList<FloorPlanDto>>;

public sealed record GetFloorPlanQuery(Guid Id) : IRequest<FloorPlanDto?>;

public sealed record DeleteFloorPlanCommand(Guid Id) : IRequest<bool>;

public sealed record GetAssetPositionsQuery(Guid FloorPlanId)
    : IRequest<IReadOnlyList<AssetPositionDto>>;

public sealed record AssetPositionInput(Guid AssetId, double X, double Y);

public sealed record ReplaceAssetPositionsCommand(
    Guid FloorPlanId,
    IReadOnlyList<AssetPositionInput> Positions) : IRequest<IReadOnlyList<AssetPositionDto>>;

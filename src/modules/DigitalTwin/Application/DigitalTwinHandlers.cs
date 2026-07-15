using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.DigitalTwin.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.DigitalTwin.Application;

public sealed class CreateFloorPlanHandler(AppDbContext db)
    : IRequestHandler<CreateFloorPlanCommand, Guid>
{
    public async Task<Guid> Handle(CreateFloorPlanCommand command, CancellationToken cancellationToken)
    {
        ValidateDimensions(command.Width, command.Height);
        var floorPlan = FloorPlan.Create(
            db.CurrentTenantId,
            command.Name,
            command.FacilityId,
            command.ImageUrl,
            command.Width,
            command.Height);
        db.FloorPlans.Add(floorPlan);
        await db.SaveChangesAsync(cancellationToken);
        return floorPlan.Id;
    }

    internal static void ValidateDimensions(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Floor plan dimensions must be greater than zero.");
    }
}

public sealed class UpdateFloorPlanHandler(AppDbContext db)
    : IRequestHandler<UpdateFloorPlanCommand, FloorPlanDto?>
{
    public async Task<FloorPlanDto?> Handle(UpdateFloorPlanCommand command, CancellationToken cancellationToken)
    {
        CreateFloorPlanHandler.ValidateDimensions(command.Width, command.Height);
        var floorPlan = await db.FloorPlans.SingleOrDefaultAsync(
            plan => plan.Id == command.Id && plan.TenantId == db.CurrentTenantId,
            cancellationToken);
        if (floorPlan is null)
            return null;

        floorPlan.Update(command.Name, command.FacilityId, command.ImageUrl, command.Width, command.Height);
        await db.SaveChangesAsync(cancellationToken);
        return floorPlan.ToDto();
    }
}

public sealed class GetFloorPlansHandler(AppDbContext db)
    : IRequestHandler<GetFloorPlansQuery, IReadOnlyList<FloorPlanDto>>
{
    public async Task<IReadOnlyList<FloorPlanDto>> Handle(
        GetFloorPlansQuery query,
        CancellationToken cancellationToken)
        => await db.FloorPlans
            .Where(plan => plan.TenantId == db.CurrentTenantId)
            .OrderByDescending(plan => plan.CreatedAt)
            .ApplyPaging(query.Skip, query.Take)
            .Select(plan => new FloorPlanDto(
                plan.Id,
                plan.Name,
                plan.FacilityId,
                plan.ImageUrl,
                plan.Width,
                plan.Height))
            .ToListAsync(cancellationToken);
}

public sealed class GetFloorPlanHandler(AppDbContext db)
    : IRequestHandler<GetFloorPlanQuery, FloorPlanDto?>
{
    public async Task<FloorPlanDto?> Handle(
        GetFloorPlanQuery query,
        CancellationToken cancellationToken)
        => await db.FloorPlans
            .Where(plan => plan.Id == query.Id && plan.TenantId == db.CurrentTenantId)
            .Select(plan => new FloorPlanDto(
                plan.Id,
                plan.Name,
                plan.FacilityId,
                plan.ImageUrl,
                plan.Width,
                plan.Height))
            .SingleOrDefaultAsync(cancellationToken);
}

public sealed class DeleteFloorPlanHandler(AppDbContext db)
    : IRequestHandler<DeleteFloorPlanCommand, bool>
{
    public async Task<bool> Handle(DeleteFloorPlanCommand command, CancellationToken cancellationToken)
    {
        var floorPlan = await db.FloorPlans.SingleOrDefaultAsync(
            plan => plan.Id == command.Id && plan.TenantId == db.CurrentTenantId,
            cancellationToken);
        if (floorPlan is null)
            return false;

        floorPlan.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class GetAssetPositionsHandler(AppDbContext db)
    : IRequestHandler<GetAssetPositionsQuery, IReadOnlyList<AssetPositionDto>>
{
    public async Task<IReadOnlyList<AssetPositionDto>> Handle(
        GetAssetPositionsQuery query,
        CancellationToken cancellationToken)
        => await (
            from position in db.AssetPositions
            join asset in db.Assets on position.AssetId equals asset.Id
            where position.FloorPlanId == query.FloorPlanId
                && position.TenantId == db.CurrentTenantId
                && asset.TenantId == db.CurrentTenantId
            orderby asset.Name
            select new AssetPositionDto(
                position.Id,
                position.AssetId,
                position.FloorPlanId,
                position.X,
                position.Y,
                asset.Name,
                asset.Criticality.ToString()))
            .ToListAsync(cancellationToken);
}

public sealed class ReplaceAssetPositionsHandler(AppDbContext db)
    : IRequestHandler<ReplaceAssetPositionsCommand, IReadOnlyList<AssetPositionDto>>
{
    public async Task<IReadOnlyList<AssetPositionDto>> Handle(
        ReplaceAssetPositionsCommand command,
        CancellationToken cancellationToken)
    {
        ValidatePositions(command.Positions);
        var floorPlanExists = await db.FloorPlans.AnyAsync(
            plan => plan.Id == command.FloorPlanId && plan.TenantId == db.CurrentTenantId,
            cancellationToken);
        if (!floorPlanExists)
            throw new KeyNotFoundException($"Floor plan {command.FloorPlanId} not found.");

        var assetIds = command.Positions.Select(position => position.AssetId).Distinct().ToList();
        var validAssetIds = await db.Assets
            .Where(asset => asset.TenantId == db.CurrentTenantId && assetIds.Contains(asset.Id))
            .Select(asset => asset.Id)
            .ToListAsync(cancellationToken);
        if (validAssetIds.Count != assetIds.Count)
            throw new KeyNotFoundException("One or more assets were not found.");

        var existing = await db.AssetPositions
            .Where(position => position.FloorPlanId == command.FloorPlanId && position.TenantId == db.CurrentTenantId)
            .ToListAsync(cancellationToken);
        var existingAssetIds = existing.Select(position => position.AssetId).ToHashSet();
        var requested = command.Positions.ToDictionary(position => position.AssetId);
        foreach (var position in existing)
        {
            if (requested.TryGetValue(position.AssetId, out var input))
                position.Move(input.X, input.Y);
            else
                position.SoftDelete();
        }

        foreach (var input in command.Positions.Where(input => !existingAssetIds.Contains(input.AssetId)))
            db.AssetPositions.Add(AssetPosition.Create(
                db.CurrentTenantId,
                input.AssetId,
                command.FloorPlanId,
                input.X,
                input.Y));

        await db.SaveChangesAsync(cancellationToken);
        return await new GetAssetPositionsHandler(db).Handle(
            new GetAssetPositionsQuery(command.FloorPlanId),
            cancellationToken);
    }

    private static void ValidatePositions(IReadOnlyList<AssetPositionInput> positions)
    {
        if (positions.Select(position => position.AssetId).Distinct().Count() != positions.Count)
            throw new ArgumentException("Each asset may have only one position per floor plan.");
        if (positions.Any(position => position.X is < 0 or > 1 || position.Y is < 0 or > 1))
            throw new ArgumentOutOfRangeException(nameof(positions), "Asset positions must be relative coordinates between 0 and 1.");
    }
}

internal static class DigitalTwinMappings
{
    public static FloorPlanDto ToDto(this FloorPlan floorPlan)
        => new(
            floorPlan.Id,
            floorPlan.Name,
            floorPlan.FacilityId,
            floorPlan.ImageUrl,
            floorPlan.Width,
            floorPlan.Height);
}

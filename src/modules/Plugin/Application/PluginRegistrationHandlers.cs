using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Plugin.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Plugin.Application;

public sealed class CreatePluginRegistrationHandler(AppDbContext db)
    : IRequestHandler<CreatePluginRegistrationCommand, Guid>
{
    public async Task<Guid> Handle(
        CreatePluginRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        var plugin = PluginRegistration.Register(
            db.CurrentTenantId,
            command.Name,
            command.Version,
            command.Description,
            command.Author,
            command.SourceUrl,
            command.StoragePath);
        db.PluginRegistrations.Add(plugin);
        await db.SaveChangesAsync(cancellationToken);
        return plugin.Id;
    }
}

public sealed class UpdatePluginRegistrationHandler(AppDbContext db)
    : IRequestHandler<UpdatePluginRegistrationCommand, PluginRegistrationDto?>
{
    public async Task<PluginRegistrationDto?> Handle(
        UpdatePluginRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        var plugin = await db.PluginRegistrations
            .SingleOrDefaultAsync(
                p => p.Id == command.Id && p.TenantId == db.CurrentTenantId,
                cancellationToken);
        if (plugin is null)
            return null;

        plugin.Update(
            command.Name,
            command.Version,
            command.Description,
            command.Author,
            command.Status,
            command.SourceUrl,
            command.StoragePath);
        await db.SaveChangesAsync(cancellationToken);
        return plugin.ToDto();
    }
}

public sealed class GetPluginRegistrationsHandler(AppDbContext db)
    : IRequestHandler<GetPluginRegistrationsQuery, IReadOnlyList<PluginRegistrationDto>>
{
    public async Task<IReadOnlyList<PluginRegistrationDto>> Handle(
        GetPluginRegistrationsQuery query,
        CancellationToken cancellationToken)
        => await db.PluginRegistrations
            .Where(p => p.TenantId == db.CurrentTenantId)
            .OrderByDescending(p => p.CreatedAt)
            .ApplyPaging(query.Skip, query.Take)
            .Select(p => new PluginRegistrationDto(
                p.Id,
                p.Name,
                p.Version,
                p.Description,
                p.Author,
                p.Status,
                p.SourceUrl,
                p.StoragePath,
                p.InstalledAt))
            .ToListAsync(cancellationToken);
}

public sealed class GetPluginRegistrationHandler(AppDbContext db)
    : IRequestHandler<GetPluginRegistrationQuery, PluginRegistrationDto?>
{
    public async Task<PluginRegistrationDto?> Handle(
        GetPluginRegistrationQuery query,
        CancellationToken cancellationToken)
        => await db.PluginRegistrations
            .Where(p => p.Id == query.Id && p.TenantId == db.CurrentTenantId)
            .Select(p => new PluginRegistrationDto(
                p.Id,
                p.Name,
                p.Version,
                p.Description,
                p.Author,
                p.Status,
                p.SourceUrl,
                p.StoragePath,
                p.InstalledAt))
            .SingleOrDefaultAsync(cancellationToken);
}

public sealed class DeletePluginRegistrationHandler(AppDbContext db)
    : IRequestHandler<DeletePluginRegistrationCommand, bool>
{
    public async Task<bool> Handle(
        DeletePluginRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        var plugin = await db.PluginRegistrations
            .SingleOrDefaultAsync(
                p => p.Id == command.Id && p.TenantId == db.CurrentTenantId,
                cancellationToken);
        if (plugin is null)
            return false;

        plugin.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal static class PluginRegistrationMappings
{
    public static PluginRegistrationDto ToDto(this PluginRegistration plugin)
        => new(
            plugin.Id,
            plugin.Name,
            plugin.Version,
            plugin.Description,
            plugin.Author,
            plugin.Status,
            plugin.SourceUrl,
            plugin.StoragePath,
            plugin.InstalledAt);
}

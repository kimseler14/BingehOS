using BingehOS.Modules.Plugin.Domain;
using MediatR;

namespace BingehOS.Modules.Plugin.Application;

public record PluginRegistrationDto(
    Guid Id,
    string Name,
    string Version,
    string? Description,
    string? Author,
    PluginStatus Status,
    string? SourceUrl,
    string? StoragePath,
    DateTimeOffset? InstalledAt);

public record CreatePluginRegistrationCommand(
    string Name,
    string Version,
    string? Description,
    string? Author,
    string? SourceUrl,
    string? StoragePath) : IRequest<Guid>;

public record UpdatePluginRegistrationCommand(
    Guid Id,
    string Name,
    string Version,
    string? Description,
    string? Author,
    PluginStatus Status,
    string? SourceUrl,
    string? StoragePath) : IRequest<PluginRegistrationDto?>;

public record GetPluginRegistrationsQuery(int Skip = 0, int Take = 50)
    : IRequest<IReadOnlyList<PluginRegistrationDto>>;

public record GetPluginRegistrationQuery(Guid Id) : IRequest<PluginRegistrationDto?>;

public record DeletePluginRegistrationCommand(Guid Id) : IRequest<bool>;

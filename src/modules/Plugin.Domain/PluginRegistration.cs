using BingehOS.Shared;
using System.Text.Json.Serialization;

namespace BingehOS.Modules.Plugin.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PluginStatus
{
    Available,
    Enabled,
    Disabled
}

public sealed class PluginRegistration : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Version { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Author { get; private set; }
    public PluginStatus Status { get; private set; } = PluginStatus.Available;
    public string? SourceUrl { get; private set; }
    public string? StoragePath { get; private set; }
    public DateTimeOffset? InstalledAt { get; private set; }

    public static PluginRegistration Register(
        Guid tenantId,
        string name,
        string version,
        string? description,
        string? author,
        string? sourceUrl,
        string? storagePath)
        => new()
        {
            TenantId = tenantId,
            Name = name,
            Version = version,
            Description = description,
            Author = author,
            SourceUrl = sourceUrl,
            StoragePath = storagePath,
            Status = PluginStatus.Available
        };

    public void Update(
        string name,
        string version,
        string? description,
        string? author,
        PluginStatus status,
        string? sourceUrl,
        string? storagePath)
    {
        Name = name;
        Version = version;
        Description = description;
        Author = author;
        SourceUrl = sourceUrl;
        StoragePath = storagePath;
        SetStatus(status);
    }

    public void SetStatus(PluginStatus status)
    {
        Status = status;
        if (status == PluginStatus.Enabled && InstalledAt is null)
            InstalledAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Status = PluginStatus.Disabled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

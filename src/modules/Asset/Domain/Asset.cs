using BingehOS.Shared;

namespace BingehOS.Modules.Asset.Domain;

public enum AssetCriticality
{
    Low, Normal, High, Critical
}

public class Asset : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? SerialNumber { get; private set; }
    public string? LocationCode { get; private set; }
    public AssetCriticality Criticality { get; private set; } = AssetCriticality.Normal;

    public static Asset Create(Guid tenantId, string name, string? serialNumber, string? locationCode, AssetCriticality criticality)
        => new() { TenantId = tenantId, Name = name, SerialNumber = serialNumber, LocationCode = locationCode, Criticality = criticality };

    public void Rename(string name) => Name = name;
    public void ChangeLocation(string? locationCode) => LocationCode = locationCode;
    public void SetCriticality(AssetCriticality criticality) => Criticality = criticality;
}

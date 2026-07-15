using BingehOS.Modules.Asset.Domain;

namespace BingehOS.UnitTests.Domain;

public class AssetDomainTests
{
    [Fact]
    public void AssetClass_Create_And_Rename()
    {
        var tenant = Guid.NewGuid();
        var ac = AssetClass.Create(tenant, "Pumps", "Rotating equipment", "pump-icon");

        Assert.Equal(tenant, ac.TenantId);
        Assert.Equal("Pumps", ac.Name);
        Assert.Equal("Rotating equipment", ac.Description);
        Assert.Equal("pump-icon", ac.Icon);
        Assert.Empty(ac.AssetTypes);

        ac.Rename("Rotating");
        Assert.Equal("Rotating", ac.Name);
    }

    [Fact]
    public void AssetType_Create_And_Rename()
    {
        var tenant = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var at = AssetType.Create(tenant, classId, "Centrifugal Pump", "desc", "M-100", "Acme", 15);

        Assert.Equal(tenant, at.TenantId);
        Assert.Equal(classId, at.AssetClassId);
        Assert.Equal("Centrifugal Pump", at.Name);
        Assert.Equal("desc", at.Description);
        Assert.Equal("M-100", at.Model);
        Assert.Equal("Acme", at.Manufacturer);
        Assert.Equal(15, at.ExpectedLifespanYears);
        Assert.Empty(at.AssetTemplates);

        at.Rename("Pump");
        Assert.Equal("Pump", at.Name);
    }

    [Fact]
    public void AssetTemplate_Create_Defaults_And_Rename()
    {
        var tenant = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var tpl = AssetTemplate.Create(tenant, typeId, "Standard Pump", "d", "{\"k\":1}");

        Assert.Equal(tenant, tpl.TenantId);
        Assert.Equal(typeId, tpl.AssetTypeId);
        Assert.Equal("Standard Pump", tpl.Name);
        Assert.Equal("{\"k\":1}", tpl.Attributes);
        Assert.False(tpl.IsPreseeded);

        var seeded = AssetTemplate.Create(tenant, typeId, "Seed", null, null, isPreseeded: true);
        Assert.True(seeded.IsPreseeded);

        tpl.Rename("Custom Pump");
        Assert.Equal("Custom Pump", tpl.Name);
    }

    [Fact]
    public void AssetRelationship_Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var parent = Guid.NewGuid();
        var child = Guid.NewGuid();
        var rel = AssetRelationship.Create(tenant, parent, child, "Contains", "note");

        Assert.Equal(tenant, rel.TenantId);
        Assert.Equal(parent, rel.ParentAssetId);
        Assert.Equal(child, rel.ChildAssetId);
        Assert.Equal("Contains", rel.RelationshipType);
        Assert.Equal("note", rel.Description);
    }

    [Fact]
    public void Meter_Create_And_RecordReading()
    {
        var tenant = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var meter = Meter.Create(tenant, assetId, "Runtime", "hours", "Usage");

        Assert.Equal(assetId, meter.AssetId);
        Assert.Equal("Runtime", meter.Name);
        Assert.Equal("hours", meter.Unit);
        Assert.Equal("Usage", meter.MeterType);
        Assert.Null(meter.LastReadingAt);
        Assert.Null(meter.LastReadingValue);

        var before = DateTimeOffset.UtcNow;
        meter.RecordReading(123.5);
        var after = DateTimeOffset.UtcNow;
        Assert.Equal(123.5, meter.LastReadingValue);
        Assert.NotNull(meter.LastReadingAt);
        Assert.InRange(meter.LastReadingAt.Value, before, after);
    }

    [Fact]
    public void Warranty_Create_And_IsActive_Boundaries()
    {
        var tenant = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var start = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2024, 12, 31, 0, 0, 0, TimeSpan.Zero);
        var w = Warranty.Create(tenant, assetId, "Acme", start, end, "http://doc", "notes");

        Assert.Equal(assetId, w.AssetId);
        Assert.Equal("Acme", w.Provider);
        Assert.Equal(start, w.StartDate);
        Assert.Equal(end, w.EndDate);

        Assert.True(w.IsActive(start));
        Assert.True(w.IsActive(end));
        Assert.True(w.IsActive(new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero)));
        Assert.False(w.IsActive(start.AddDays(-1)));
        Assert.False(w.IsActive(end.AddDays(1)));
    }

    [Fact]
    public void AssetHealthScore_Create_And_UpdateScore()
    {
        var tenant = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var score = AssetHealthScore.Create(tenant, assetId, 80, "initial");

        Assert.Equal(assetId, score.AssetId);
        Assert.Equal(80, score.Score);
        Assert.Equal("initial", score.CalculationDetails);
        var firstCalc = score.CalculatedAt;
        Assert.NotEqual(default, firstCalc);

        var beforeUpdate = DateTimeOffset.UtcNow;
        score.UpdateScore(55, "degraded");
        var afterUpdate = DateTimeOffset.UtcNow;
        Assert.Equal(55, score.Score);
        Assert.Equal("degraded", score.CalculationDetails);
        Assert.InRange(score.CalculatedAt, beforeUpdate, afterUpdate);
    }
}

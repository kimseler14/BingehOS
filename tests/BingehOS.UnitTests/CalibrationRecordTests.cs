using BingehOS.Modules.Compliance.Domain;

namespace BingehOS.UnitTests;

public class CalibrationRecordTests
{
    [Fact]
    public void Create_SetsCalibrationFields()
    {
        var tenantId = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var calibratedAt = new DateTimeOffset(2026, 1, 15, 8, 30, 0, TimeSpan.Zero);
        var nextDueAt = calibratedAt.AddMonths(12);

        var record = CalibrationRecord.Create(
            tenantId,
            assetId,
            calibratedAt,
            nextDueAt,
            "Uygun");

        Assert.Equal(tenantId, record.TenantId);
        Assert.Equal(assetId, record.AssetId);
        Assert.Equal(calibratedAt, record.CalibratedAt);
        Assert.Equal(nextDueAt, record.NextDueAt);
        Assert.Equal("Uygun", record.Result);
    }

    [Fact]
    public void Update_ChangesCalibrationFields()
    {
        var record = CalibrationRecord.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            null,
            "Beklemede");
        var assetId = Guid.NewGuid();
        var calibratedAt = DateTimeOffset.UtcNow.AddDays(1);

        record.Update(assetId, calibratedAt, calibratedAt.AddMonths(6), "Uygun");

        Assert.Equal(assetId, record.AssetId);
        Assert.Equal(calibratedAt, record.CalibratedAt);
        Assert.Equal("Uygun", record.Result);
    }
}

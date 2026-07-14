using BingehOS.Modules.Personnel.Domain;

namespace BingehOS.UnitTests;

public class WorkerTests
{
    [Fact]
    public void Create_Sets_Worker_Fields()
    {
        var tenantId = Guid.NewGuid();
        var worker = Worker.Create(
            tenantId,
            "Ali",
            "Veli",
            "WRK-1",
            "Electrician",
            "Maintenance",
            "+90 555 000 00 00",
            true);

        Assert.Equal(tenantId, worker.TenantId);
        Assert.Equal("Ali", worker.FirstName);
        Assert.Equal("Veli", worker.LastName);
        Assert.Equal("WRK-1", worker.EmployeeNumber);
        Assert.Equal("Electrician", worker.Trade);
        Assert.Equal("Maintenance", worker.Department);
        Assert.Equal("+90 555 000 00 00", worker.Phone);
        Assert.True(worker.IsActive);
    }

    [Fact]
    public void Update_And_Status_Changes_Work()
    {
        var worker = Worker.Create(Guid.NewGuid(), "Ali", "Veli", null, null, null, null, true);

        worker.Update("Ayse", "Yilmaz", "WRK-2", "Welder", "Production", "+90 555 111 11 11");
        worker.Deactivate();

        Assert.Equal("Ayse", worker.FirstName);
        Assert.Equal("Yilmaz", worker.LastName);
        Assert.Equal("WRK-2", worker.EmployeeNumber);
        Assert.Equal("Welder", worker.Trade);
        Assert.Equal("Production", worker.Department);
        Assert.False(worker.IsActive);

        worker.Activate();
        Assert.True(worker.IsActive);
    }
}
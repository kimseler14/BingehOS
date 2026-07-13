using FacilityOS.Modules.Personnel.Domain;

namespace FacilityOS.UnitTests;

public class WorkerTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var worker = Worker.Create(Guid.NewGuid(), "Ali", "Veli", "EMP-1", "Maintenance", "+90 555 000 00 00", true);
        Assert.Equal("Ali", worker.FirstName);
        Assert.Equal("Veli", worker.LastName);
        Assert.Equal("EMP-1", worker.EmployeeNumber);
    }

    [Fact]
    public void Deactivate_And_Activate_Work()
    {
        var worker = Worker.Create(Guid.NewGuid(), "Ali", "Veli", null, null, null, true);
        worker.Deactivate();
        Assert.False(worker.IsActive);
        worker.Activate();
        Assert.True(worker.IsActive);
    }
}

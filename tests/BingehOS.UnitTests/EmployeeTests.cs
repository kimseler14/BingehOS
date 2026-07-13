using BingehOS.Modules.Personnel.Domain;

namespace BingehOS.UnitTests;

public class EmployeeTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var employee = Employee.Create(Guid.NewGuid(), "Ali", "Veli", "EMP-1", "Maintenance", "+90 555 000 00 00", true);
        Assert.Equal("Ali", employee.FirstName);
        Assert.Equal("Veli", employee.LastName);
        Assert.Equal("EMP-1", employee.EmployeeNumber);
    }

    [Fact]
    public void Deactivate_And_Activate_Work()
    {
        var employee = Employee.Create(Guid.NewGuid(), "Ali", "Veli", null, null, null, true);
        employee.Deactivate();
        Assert.False(employee.IsActive);
        employee.Activate();
        Assert.True(employee.IsActive);
    }
}

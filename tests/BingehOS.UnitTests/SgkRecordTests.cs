using BingehOS.Modules.Personnel.Domain;

namespace BingehOS.UnitTests;

public class SgkRecordTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var employeeId = Guid.NewGuid();
        var record = SgkRecord.Create(Guid.NewGuid(), employeeId, "SGK-123", "PRF-001", "NACE-01", DateTime.UtcNow);
        Assert.Equal(employeeId, record.EmployeeId);
        Assert.Equal("SGK-123", record.SgkNumber);
        Assert.Equal("PRF-001", record.ProfessionCode);
        Assert.Null(record.TerminationDate);
    }

    [Fact]
    public void Terminate_Sets_TerminationDate()
    {
        var record = SgkRecord.Create(Guid.NewGuid(), Guid.NewGuid(), "SGK-123", "PRF-001", "NACE-01", DateTime.UtcNow);
        var terminationDate = DateTime.UtcNow;
        record.Terminate(terminationDate);
        Assert.Equal(terminationDate, record.TerminationDate);
    }
}

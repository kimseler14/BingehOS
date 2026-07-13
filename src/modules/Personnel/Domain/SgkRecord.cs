using BingehOS.Shared;

namespace BingehOS.Modules.Personnel.Domain;

public class SgkRecord : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public string SgkNumber { get; private set; } = string.Empty;
    public string ProfessionCode { get; private set; } = string.Empty;
    public string NaceCode { get; private set; } = string.Empty;
    public DateTime RegistrationDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }

    public static SgkRecord Create(Guid tenantId, Guid employeeId, string sgkNumber, string professionCode, string naceCode, DateTime registrationDate)
        => new() { TenantId = tenantId, EmployeeId = employeeId, SgkNumber = sgkNumber, ProfessionCode = professionCode, NaceCode = naceCode, RegistrationDate = registrationDate };

    public void Terminate(DateTime terminationDate) => TerminationDate = terminationDate;
}

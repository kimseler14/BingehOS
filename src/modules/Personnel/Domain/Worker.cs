using BingehOS.Shared;

namespace BingehOS.Modules.Personnel.Domain;

public class Worker : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? EmployeeNumber { get; private set; }
    public string? Department { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static Worker Create(Guid tenantId, string firstName, string lastName, string? employeeNumber, string? department, string? phone, bool isActive)
        => new() { TenantId = tenantId, FirstName = firstName, LastName = lastName, EmployeeNumber = employeeNumber, Department = department, Phone = phone, IsActive = isActive };

    public void Rename(string firstName, string lastName) { FirstName = firstName; LastName = lastName; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

using BingehOS.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Personnel.Application;

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly AppDbContext _db;
    public UpdateEmployeeHandler(AppDbContext db) => _db = db;

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = await _db.Set<Domain.Employee>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                      ?? throw new KeyNotFoundException($"Employee {cmd.Id} not found.");

        employee.Rename(cmd.FirstName, cmd.LastName);
        if (cmd.IsActive) employee.Activate(); else employee.Deactivate();

        await _db.SaveChangesAsync(ct);
        return new EmployeeDto(employee.Id, employee.FirstName, employee.LastName, employee.EmployeeNumber, employee.Department, employee.Phone, employee.IsActive);
    }
}

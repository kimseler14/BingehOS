using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Personnel.Domain;
using MediatR;

namespace BingehOS.Modules.Personnel.Application;

public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateEmployeeHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = Domain.Employee.Create(_db.CurrentTenantId, cmd.FirstName, cmd.LastName, cmd.EmployeeNumber, cmd.Department, cmd.Phone, cmd.IsActive);
        _db.Set<Domain.Employee>().Add(employee);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new EmployeeCreatedEvent(employee.Id, employee.FirstName, employee.LastName), ct);
        return employee.Id;
    }
}

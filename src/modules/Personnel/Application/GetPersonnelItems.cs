using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Personnel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Personnel.Application;

public record EmployeeListItem(Guid Id, string FirstName, string LastName, string? EmployeeNumber, string? Department, bool IsActive);
public record WorkerListItem(Guid Id, string FirstName, string LastName, string? EmployeeNumber, string? Trade, string? Department, string? Phone, bool IsActive);
public record SubcontractorListItem(Guid Id, string CompanyName, string TaxNumber, string? ContactPerson, string? Phone, bool IsActive);
public record SgkRecordListItem(Guid Id, Guid EmployeeId, string SgkNumber, string ProfessionCode, string NaceCode, DateTime RegistrationDate, DateTime? TerminationDate);

public record GetEmployeeQuery(Guid Id) : IRequest<EmployeeListItem?>;
public record GetEmployeesQuery(int Skip = 0, int Take = 20, bool? activeOnly = null) : IRequest<IReadOnlyList<EmployeeListItem>>;
public record GetWorkerQuery(Guid Id) : IRequest<WorkerListItem?>;
public record GetWorkersQuery(int Skip = 0, int Take = 20, bool? activeOnly = null) : IRequest<IReadOnlyList<WorkerListItem>>;

public record GetSubcontractorQuery(Guid Id) : IRequest<SubcontractorListItem?>;
public record GetSubcontractorsQuery(int Skip = 0, int Take = 20, bool? activeOnly = null) : IRequest<IReadOnlyList<SubcontractorListItem>>;

public record GetSgkRecordQuery(Guid Id) : IRequest<SgkRecordListItem?>;
public record GetSgkRecordsQuery(int Skip = 0, int Take = 20, Guid? employeeId = null) : IRequest<IReadOnlyList<SgkRecordListItem>>;

public class GetEmployeeHandler : IRequestHandler<GetEmployeeQuery, EmployeeListItem?>
{
    private readonly AppDbContext _db;
    public GetEmployeeHandler(AppDbContext db) => _db = db;

    public async Task<EmployeeListItem?> Handle(GetEmployeeQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<Employee>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new EmployeeListItem(entity.Id, entity.FirstName, entity.LastName, entity.EmployeeNumber, entity.Department, entity.IsActive);
    }
}

public class GetWorkerHandler : IRequestHandler<GetWorkerQuery, WorkerListItem?>
{
    private readonly AppDbContext _db;
    public GetWorkerHandler(AppDbContext db) => _db = db;

    public async Task<WorkerListItem?> Handle(GetWorkerQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<Worker>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new WorkerListItem(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.EmployeeNumber,
            entity.Trade,
            entity.Department,
            entity.Phone,
            entity.IsActive);
    }
}

public class GetWorkersHandler : IRequestHandler<GetWorkersQuery, IReadOnlyList<WorkerListItem>>
{
    private readonly AppDbContext _db;
    public GetWorkersHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<WorkerListItem>> Handle(GetWorkersQuery q, CancellationToken ct)
    {
        var query = _db.Set<Worker>().AsQueryable();
        if (q.activeOnly.HasValue) query = query.Where(e => e.IsActive == q.activeOnly.Value);

        return await query.OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new WorkerListItem(
                e.Id,
                e.FirstName,
                e.LastName,
                e.EmployeeNumber,
                e.Trade,
                e.Department,
                e.Phone,
                e.IsActive))
            .ToListAsync(ct);
    }
}

public class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, IReadOnlyList<EmployeeListItem>>
{
    private readonly AppDbContext _db;
    public GetEmployeesHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<EmployeeListItem>> Handle(GetEmployeesQuery q, CancellationToken ct)
    {
        var query = _db.Set<Employee>().AsQueryable();
        if (q.activeOnly.HasValue) query = query.Where(e => e.IsActive == q.activeOnly.Value);

        return await query.OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new EmployeeListItem(e.Id, e.FirstName, e.LastName, e.EmployeeNumber, e.Department, e.IsActive))
            .ToListAsync(ct);
    }
}

public class GetSubcontractorHandler : IRequestHandler<GetSubcontractorQuery, SubcontractorListItem?>
{
    private readonly AppDbContext _db;
    public GetSubcontractorHandler(AppDbContext db) => _db = db;

    public async Task<SubcontractorListItem?> Handle(GetSubcontractorQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<Subcontractor>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new SubcontractorListItem(entity.Id, entity.CompanyName, entity.TaxNumber, entity.ContactPerson, entity.Phone, entity.IsActive);
    }
}

public class GetSubcontractorsHandler : IRequestHandler<GetSubcontractorsQuery, IReadOnlyList<SubcontractorListItem>>
{
    private readonly AppDbContext _db;
    public GetSubcontractorsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<SubcontractorListItem>> Handle(GetSubcontractorsQuery q, CancellationToken ct)
    {
        var query = _db.Set<Subcontractor>().AsQueryable();
        if (q.activeOnly.HasValue) query = query.Where(e => e.IsActive == q.activeOnly.Value);

        return await query.OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new SubcontractorListItem(e.Id, e.CompanyName, e.TaxNumber, e.ContactPerson, e.Phone, e.IsActive))
            .ToListAsync(ct);
    }
}

public class GetSgkRecordHandler : IRequestHandler<GetSgkRecordQuery, SgkRecordListItem?>
{
    private readonly AppDbContext _db;
    public GetSgkRecordHandler(AppDbContext db) => _db = db;

    public async Task<SgkRecordListItem?> Handle(GetSgkRecordQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<SgkRecord>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new SgkRecordListItem(entity.Id, entity.EmployeeId, entity.SgkNumber, entity.ProfessionCode, entity.NaceCode, entity.RegistrationDate, entity.TerminationDate);
    }
}

public class GetSgkRecordsHandler : IRequestHandler<GetSgkRecordsQuery, IReadOnlyList<SgkRecordListItem>>
{
    private readonly AppDbContext _db;
    public GetSgkRecordsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<SgkRecordListItem>> Handle(GetSgkRecordsQuery q, CancellationToken ct)
    {
        var query = _db.Set<SgkRecord>().AsQueryable();
        if (q.employeeId.HasValue) query = query.Where(e => e.EmployeeId == q.employeeId.Value);

        return await query.OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new SgkRecordListItem(e.Id, e.EmployeeId, e.SgkNumber, e.ProfessionCode, e.NaceCode, e.RegistrationDate, e.TerminationDate))
            .ToListAsync(ct);
    }
}

using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Finance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Finance.Application;

public record CostCenterListItem(Guid Id, string Code, string Name, Guid? ParentCostCenterId, long BudgetMinor, string Currency, bool IsActive);
public record InvoiceListItem(Guid Id, string InvoiceNumber, DateTimeOffset InvoiceDate, DateTimeOffset DueDate, long TotalAmountMinor, string Currency, string Status, string Type);
public record TaxRecordListItem(Guid Id, Guid InvoiceId, string TaxType, decimal TaxRate, long TaxAmountMinor, string Currency);

public record GetCostCenterQuery(Guid Id) : IRequest<CostCenterListItem?>;
public record GetCostCentersQuery(int Skip = 0, int Take = 20, bool? activeOnly = null) : IRequest<IReadOnlyList<CostCenterListItem>>;

public record GetInvoiceQuery(Guid Id) : IRequest<InvoiceListItem?>;
public record GetInvoicesQuery(int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<InvoiceListItem>>;

public record GetTaxRecordQuery(Guid Id) : IRequest<TaxRecordListItem?>;
public record GetTaxRecordsQuery(int Skip = 0, int Take = 20, Guid? invoiceId = null) : IRequest<IReadOnlyList<TaxRecordListItem>>;

public class GetCostCenterHandler : IRequestHandler<GetCostCenterQuery, CostCenterListItem?>
{
    private readonly AppDbContext _db;
    public GetCostCenterHandler(AppDbContext db) => _db = db;

    public async Task<CostCenterListItem?> Handle(GetCostCenterQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<CostCenter>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new CostCenterListItem(entity.Id, entity.Code, entity.Name, entity.ParentCostCenterId, entity.BudgetMinor, entity.Currency, entity.IsActive);
    }
}

public class GetCostCentersHandler : IRequestHandler<GetCostCentersQuery, IReadOnlyList<CostCenterListItem>>
{
    private readonly AppDbContext _db;
    public GetCostCentersHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<CostCenterListItem>> Handle(GetCostCentersQuery q, CancellationToken ct)
    {
        var query = _db.Set<CostCenter>().AsQueryable();
        if (q.activeOnly.HasValue) query = query.Where(e => e.IsActive == q.activeOnly.Value);

        return await query.OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new CostCenterListItem(e.Id, e.Code, e.Name, e.ParentCostCenterId, e.BudgetMinor, e.Currency, e.IsActive))
            .ToListAsync(ct);
    }
}

public class GetInvoiceHandler : IRequestHandler<GetInvoiceQuery, InvoiceListItem?>
{
    private readonly AppDbContext _db;
    public GetInvoiceHandler(AppDbContext db) => _db = db;

    public async Task<InvoiceListItem?> Handle(GetInvoiceQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<Invoice>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new InvoiceListItem(entity.Id, entity.InvoiceNumber, entity.InvoiceDate, entity.DueDate, entity.TotalAmountMinor, entity.Currency, entity.Status, entity.Type);
    }
}

public class GetInvoicesHandler : IRequestHandler<GetInvoicesQuery, IReadOnlyList<InvoiceListItem>>
{
    private readonly AppDbContext _db;
    public GetInvoicesHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<InvoiceListItem>> Handle(GetInvoicesQuery q, CancellationToken ct)
    {
        return await _db.Set<Invoice>().OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new InvoiceListItem(e.Id, e.InvoiceNumber, e.InvoiceDate, e.DueDate, e.TotalAmountMinor, e.Currency, e.Status, e.Type))
            .ToListAsync(ct);
    }
}

public class GetTaxRecordHandler : IRequestHandler<GetTaxRecordQuery, TaxRecordListItem?>
{
    private readonly AppDbContext _db;
    public GetTaxRecordHandler(AppDbContext db) => _db = db;

    public async Task<TaxRecordListItem?> Handle(GetTaxRecordQuery q, CancellationToken ct)
    {
        var entity = await _db.Set<TaxRecord>().FirstOrDefaultAsync(e => e.Id == q.Id, ct);
        if (entity == null) return null;
        return new TaxRecordListItem(entity.Id, entity.InvoiceId, entity.TaxType, entity.TaxRate, entity.TaxAmountMinor, entity.Currency);
    }
}

public class GetTaxRecordsHandler : IRequestHandler<GetTaxRecordsQuery, IReadOnlyList<TaxRecordListItem>>
{
    private readonly AppDbContext _db;
    public GetTaxRecordsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<TaxRecordListItem>> Handle(GetTaxRecordsQuery q, CancellationToken ct)
    {
        var query = _db.Set<TaxRecord>().AsQueryable();
        if (q.invoiceId.HasValue) query = query.Where(e => e.InvoiceId == q.invoiceId.Value);

        return await query.OrderByDescending(e => e.CreatedAt).ApplyPaging(q.Skip, q.Take)
            .Select(e => new TaxRecordListItem(e.Id, e.InvoiceId, e.TaxType, e.TaxRate, e.TaxAmountMinor, e.Currency))
            .ToListAsync(ct);
    }
}

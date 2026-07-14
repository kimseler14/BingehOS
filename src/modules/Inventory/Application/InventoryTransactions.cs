using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Automation.Application;
using BingehOS.Modules.Automation.Domain;
using BingehOS.Modules.Inventory.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Inventory.Application;

public record InventoryTransactionDto(
    Guid Id,
    Guid PartId,
    string PartNumber,
    string PartName,
    TransactionType Type,
    int Quantity,
    string UnitOfMeasure,
    Guid? BinId,
    Guid? RelatedWorkOrderId,
    string? Notes,
    DateTimeOffset TransactionDate,
    int? CurrentStock = null);

public record GetInventoryTransactionsQuery(int Skip = 0, int Take = 20, Guid? PartId = null) : IRequest<IReadOnlyList<InventoryTransactionDto>>;

public record ReceivePartCommand(Guid PartId, int Quantity, Guid? BinId = null, Guid? RelatedWorkOrderId = null, string? Notes = null) : IRequest<InventoryTransactionDto>;
public record IssuePartCommand(Guid PartId, int Quantity, Guid? BinId = null, Guid? RelatedWorkOrderId = null, string? Notes = null) : IRequest<InventoryTransactionDto>;
public record ReturnPartCommand(Guid PartId, int Quantity, Guid? BinId = null, Guid? RelatedWorkOrderId = null, string? Notes = null) : IRequest<InventoryTransactionDto>;

public static class InventoryStockCalculator
{
    public static int Apply(int currentStock, TransactionType type, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        return type switch
        {
            TransactionType.Receiving => checked(currentStock + quantity),
            TransactionType.Return => checked(currentStock + quantity),
            TransactionType.Issue => Issue(currentStock, quantity),
            _ => throw new NotSupportedException($"Transaction type {type} is not supported.")
        };
    }

    public static int Issue(int currentStock, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        var next = currentStock - quantity;
        if (next < 0)
            throw new InvalidOperationException("Insufficient stock.");

        return next;
    }
}

internal static class InventoryTransactionMapper
{
    public static InventoryTransactionDto ToDto(InventoryTransaction transaction, Part part, int? currentStock = null) =>
        new(
            transaction.Id,
            transaction.PartId,
            part.PartNumber,
            part.Name,
            transaction.Type,
            transaction.Quantity,
            transaction.UnitOfMeasure,
            transaction.BinId,
            transaction.RelatedWorkOrderId,
            transaction.Notes,
            transaction.TransactionDate,
            currentStock);
}

public abstract class InventoryTransactionHandlerBase
{
    protected readonly AppDbContext Db;
    private readonly IPublisher _publisher;

    protected InventoryTransactionHandlerBase(AppDbContext db, IPublisher publisher)
    {
        Db = db;
        _publisher = publisher;
    }

    protected async Task<InventoryTransactionDto> CreateAsync(
        Guid partId,
        TransactionType type,
        int quantity,
        Guid? binId,
        Guid? relatedWorkOrderId,
        string? notes,
        CancellationToken ct)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        await using var dbTransaction = await Db.Database.BeginTransactionAsync(ct);
        InventoryTransactionDto result;
        string partNumber;
        try
        {
            var part = await Db.Set<Part>()
                .FromSqlInterpolated($@"SELECT * FROM ""Parts"" WHERE ""Id"" = {partId} AND ""TenantId"" = {Db.CurrentTenantId} AND ""IsDeleted"" = FALSE FOR UPDATE")
                .AsTracking()
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException($"Part {partId} not found.");

            var currentStock = await GetCurrentStockAsync(partId, ct);
            var nextStock = InventoryStockCalculator.Apply(currentStock, type, quantity);

            var transaction = InventoryTransaction.Create(
                Db.CurrentTenantId,
                partId,
                binId,
                type,
                quantity,
                part.UnitOfMeasure,
                relatedWorkOrderId,
                null,
                notes);

            Db.Set<InventoryTransaction>().Add(transaction);
            await Db.SaveChangesAsync(ct);
            await dbTransaction.CommitAsync(ct);
            partNumber = part.PartNumber;
            result = InventoryTransactionMapper.ToDto(transaction, part, nextStock);
        }
        catch
        {
            await dbTransaction.RollbackAsync(ct);
            throw;
        }

        await _publisher.Publish(new AutomationTriggerNotification(
            Db.CurrentTenantId,
            AutomationTriggerType.InventoryStockLow,
            new Dictionary<string, object?>
            {
                ["partId"] = partId,
                ["partNumber"] = partNumber,
                ["currentStock"] = result.CurrentStock,
                ["quantity"] = quantity,
                ["transactionType"] = type.ToString()
            }), ct);

        return result;
    }

    protected async Task<int> GetCurrentStockAsync(Guid partId, CancellationToken ct)
    {
        var transactions = await Db.Set<InventoryTransaction>()
            .Where(t => t.PartId == partId && t.TenantId == Db.CurrentTenantId && !t.IsDeleted)
            .OrderBy(t => t.TransactionDate)
            .ThenBy(t => t.CreatedAt)
            .Select(t => new { t.Type, t.Quantity })
            .ToListAsync(ct);

        var stock = 0;
        foreach (var tx in transactions)
        {
            stock = tx.Type switch
            {
                TransactionType.Receiving => checked(stock + tx.Quantity),
                TransactionType.Return => checked(stock + tx.Quantity),
                TransactionType.Issue => InventoryStockCalculator.Issue(stock, tx.Quantity),
                _ => throw new NotSupportedException($"Transaction type {tx.Type} is not supported.")
            };
        }

        return stock;
    }
}

public class GetInventoryTransactionsHandler : IRequestHandler<GetInventoryTransactionsQuery, IReadOnlyList<InventoryTransactionDto>>
{
    private readonly AppDbContext _db;
    public GetInventoryTransactionsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<InventoryTransactionDto>> Handle(GetInventoryTransactionsQuery q, CancellationToken ct)
    {
        var transactions = await _db.Set<InventoryTransaction>()
            .Where(t => t.TenantId == _db.CurrentTenantId && !t.IsDeleted && (!q.PartId.HasValue || t.PartId == q.PartId))
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .ToListAsync(ct);

        var partIds = transactions.Select(t => t.PartId).Distinct().ToList();
        var parts = await _db.Set<Part>()
            .Where(p => partIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        return transactions
            .Where(t => parts.ContainsKey(t.PartId))
            .Select(t => InventoryTransactionMapper.ToDto(t, parts[t.PartId]))
            .ToList();
    }
}

public class ReceivePartHandler : InventoryTransactionHandlerBase, IRequestHandler<ReceivePartCommand, InventoryTransactionDto>
{
    public ReceivePartHandler(AppDbContext db, IPublisher publisher) : base(db, publisher) { }

    public Task<InventoryTransactionDto> Handle(ReceivePartCommand cmd, CancellationToken ct) =>
        CreateAsync(cmd.PartId, TransactionType.Receiving, cmd.Quantity, cmd.BinId, cmd.RelatedWorkOrderId, cmd.Notes, ct);
}

public class IssuePartHandler : InventoryTransactionHandlerBase, IRequestHandler<IssuePartCommand, InventoryTransactionDto>
{
    public IssuePartHandler(AppDbContext db, IPublisher publisher) : base(db, publisher) { }

    public Task<InventoryTransactionDto> Handle(IssuePartCommand cmd, CancellationToken ct) =>
        CreateAsync(cmd.PartId, TransactionType.Issue, cmd.Quantity, cmd.BinId, cmd.RelatedWorkOrderId, cmd.Notes, ct);
}

public class ReturnPartHandler : InventoryTransactionHandlerBase, IRequestHandler<ReturnPartCommand, InventoryTransactionDto>
{
    public ReturnPartHandler(AppDbContext db, IPublisher publisher) : base(db, publisher) { }

    public Task<InventoryTransactionDto> Handle(ReturnPartCommand cmd, CancellationToken ct) =>
        CreateAsync(cmd.PartId, TransactionType.Return, cmd.Quantity, cmd.BinId, cmd.RelatedWorkOrderId, cmd.Notes, ct);
}

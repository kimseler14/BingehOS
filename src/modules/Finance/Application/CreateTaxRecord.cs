using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public class CreateTaxRecordHandler : IRequestHandler<CreateTaxRecordCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateTaxRecordHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateTaxRecordCommand cmd, CancellationToken ct)
    {
        var taxRecord = Domain.TaxRecord.Create(_db.CurrentTenantId, cmd.InvoiceId, cmd.TaxType, cmd.TaxRate, cmd.TaxAmountMinor, cmd.Currency);
        _db.Set<Domain.TaxRecord>().Add(taxRecord);
        await _db.SaveChangesAsync(ct);
        return taxRecord.Id;
    }
}

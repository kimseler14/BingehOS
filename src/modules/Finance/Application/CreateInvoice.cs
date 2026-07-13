using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateInvoiceHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand cmd, CancellationToken ct)
    {
        var invoice = Domain.Invoice.Create(_db.CurrentTenantId, cmd.InvoiceNumber, cmd.InvoiceDate, cmd.DueDate, cmd.VendorId, cmd.TotalAmountMinor, cmd.Currency, cmd.Status, cmd.Type);
        _db.Set<Domain.Invoice>().Add(invoice);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new InvoiceCreatedEvent(invoice.Id, invoice.InvoiceNumber), ct);
        return invoice.Id;
    }
}

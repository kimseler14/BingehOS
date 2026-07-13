using BingehOS.Infrastructure;
using BingehOS.Modules.Finance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Finance.Application;

public class UpdateInvoiceHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceDto>
{
    private readonly AppDbContext _db;
    public UpdateInvoiceHandler(AppDbContext db) => _db = db;

    public async Task<InvoiceDto> Handle(UpdateInvoiceCommand cmd, CancellationToken ct)
    {
        var invoice = await _db.Set<Domain.Invoice>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                      ?? throw new KeyNotFoundException($"Invoice {cmd.Id} not found.");

        return new InvoiceDto(invoice.Id, invoice.InvoiceNumber, invoice.InvoiceDate, invoice.DueDate, invoice.VendorId, invoice.TotalAmountMinor, invoice.Currency, invoice.Status, invoice.Type);
    }
}

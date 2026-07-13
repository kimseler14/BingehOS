using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public record TaxRecordDto(Guid Id, Guid InvoiceId, string TaxType, decimal TaxRate, long TaxAmountMinor, string Currency);

public record CreateTaxRecordCommand(Guid InvoiceId, string TaxType, decimal TaxRate, long TaxAmountMinor, string Currency) : IRequest<Guid>;

using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public record InvoiceDto(Guid Id, string InvoiceNumber, DateTimeOffset InvoiceDate, DateTimeOffset DueDate, Guid? VendorId, long TotalAmountMinor, string Currency, string Status, string Type);

public record CreateInvoiceCommand(string InvoiceNumber, DateTimeOffset InvoiceDate, DateTimeOffset DueDate, Guid? VendorId, long TotalAmountMinor, string Currency, string Status, string Type) : IRequest<Guid>;

public record UpdateInvoiceCommand(Guid Id, string InvoiceNumber, DateTimeOffset InvoiceDate, DateTimeOffset DueDate, Guid? VendorId, long TotalAmountMinor, string Currency, string Status, string Type) : IRequest<InvoiceDto>;

public record MarkInvoicePaidCommand(Guid Id) : IRequest<InvoiceDto>;

using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public record InvoiceCreatedEvent(Guid InvoiceId, string InvoiceNumber) : IEvent;

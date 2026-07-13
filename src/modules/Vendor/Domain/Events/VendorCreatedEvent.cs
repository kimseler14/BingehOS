using BingehOS.Shared;

namespace BingehOS.Modules.Vendor.Domain;

public record VendorCreatedEvent(Guid VendorId, string Name) : IEvent;

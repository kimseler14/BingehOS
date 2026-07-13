using MediatR;

namespace FacilityOS.Modules.Vendor.Application;

public record VendorDto(Guid Id, string Name, string? TaxNumber, string? ContactEmail, string? Phone, bool IsActive);

public record CreateVendorCommand(string Name, string? TaxNumber, string? ContactEmail, string? Phone, bool IsActive) : IRequest<Guid>;

public record UpdateVendorCommand(Guid Id, string Name, string? ContactEmail, string? Phone, bool IsActive) : IRequest<VendorDto>;

using MediatR;

namespace FacilityOS.Modules.Facility.Application;

public record FacilityDto(Guid Id, string Name, string? Code, string? Address, string? TimeZone, Guid? ParentFacilityId);

public record CreateFacilityCommand(string Name, string? Code, string? Address, string? TimeZone, Guid? ParentFacilityId) : IRequest<Guid>;

public record UpdateFacilityCommand(Guid Id, string Name, string? Address, string? TimeZone, Guid? ParentFacilityId) : IRequest<FacilityDto>;

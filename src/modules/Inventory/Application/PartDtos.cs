using MediatR;

namespace BingehOS.Modules.Inventory.Application;

public record PartDto(Guid Id, string PartNumber, string Name, string? Description, string UnitOfMeasure, bool IsActive);

public record CreatePartCommand(string PartNumber, string Name, string? Description, string UnitOfMeasure, bool IsActive) : IRequest<Guid>;

public record UpdatePartCommand(Guid Id, string Name, string? Description, string UnitOfMeasure, bool IsActive) : IRequest<PartDto>;

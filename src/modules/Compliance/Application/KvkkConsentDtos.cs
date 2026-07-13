using BingehOS.Modules.Compliance.Domain;
using MediatR;

namespace BingehOS.Modules.Compliance.Application;

public record KvkkConsentDto(Guid Id, Guid UserId, string ConsentType, string Version, DateTimeOffset GrantedAt, DateTimeOffset? RevokedAt);

public record CreateKvkkConsentCommand(Guid UserId, string ConsentType, string Version, string IpAddress, string SignatureHash) : IRequest<Guid>;

public record RevokeKvkkConsentCommand(Guid Id, string IpAddress, string SignatureHash) : IRequest<KvkkConsentDto>;

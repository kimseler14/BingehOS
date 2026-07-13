using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Security;
using BingehOS.Modules.Identity.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BingehOS.Modules.Identity.Application;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
        });
        return services;
    }
}

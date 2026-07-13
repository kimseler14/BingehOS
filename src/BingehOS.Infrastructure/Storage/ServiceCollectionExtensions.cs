using Microsoft.Extensions.DependencyInjection;

namespace BingehOS.Infrastructure.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMinIO(this IServiceCollection services)
    {
        services.AddSingleton<MinioClient>();
        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BingehOS.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=bingehos;Username=postgres;Password=postgres");
        return new AppDbContext(optionsBuilder.Options, new DummyTenantProvider());
    }

    private sealed class DummyTenantProvider : ITenantProvider
    {
        public Guid CurrentTenantId => Guid.Empty;
    }
}

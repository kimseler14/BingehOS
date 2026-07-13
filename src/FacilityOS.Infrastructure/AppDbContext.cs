using System.Linq.Expressions;
using FacilityOS.Modules.Maintenance.Domain;
using FacilityOS.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FacilityOS.Infrastructure;

public class AppDbContext : DbContext
{
    public Guid CurrentTenantId { get; set; } = Guid.Empty;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenant) : base(options)
    {
        CurrentTenantId = tenant.CurrentTenantId;
    }

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new TenantInterceptor(this), new TenantConnectionInterceptor(this));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global filter: never return soft-deleted rows.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(ConvertFilter(entityType.ClrType));
            }
        }
        base.OnModelCreating(modelBuilder);
    }

    private static LambdaExpression ConvertFilter(Type clrType)
    {
        var param = System.Linq.Expressions.Expression.Parameter(clrType, "e");
        var prop = System.Linq.Expressions.Expression.Property(param, nameof(BaseEntity.IsDeleted));
        var body = System.Linq.Expressions.Expression.Equal(prop, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(body, param);
    }
}

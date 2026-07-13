using System.Linq.Expressions;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.Compliance.Domain;
using BingehOS.Modules.Facility.Domain;
using BingehOS.Modules.Finance.Domain;
using BingehOS.Modules.HSE.Domain;
using BingehOS.Modules.Inventory.Domain;
using BingehOS.Modules.Maintenance.Domain;
using BingehOS.Modules.Personnel.Domain;
using BingehOS.Modules.Vendor.Domain;
using BingehOS.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BingehOS.Infrastructure;

public class AppDbContext : DbContext
{
    public Guid CurrentTenantId { get; set; } = Guid.Empty;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenant) : base(options)
    {
        CurrentTenantId = tenant.CurrentTenantId;
    }

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<SgkRecord> SgkRecords => Set<SgkRecord>();
    public DbSet<Subcontractor> Subcontractors => Set<Subcontractor>();
    public DbSet<WorkOrderCost> WorkOrderCosts => Set<WorkOrderCost>();
    public DbSet<ComplianceRecord> ComplianceRecords => Set<ComplianceRecord>();
    public DbSet<KvkkConsent> KvkkConsents => Set<KvkkConsent>();
    public DbSet<JobPlanTemplate> JobPlanTemplates => Set<JobPlanTemplate>();

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

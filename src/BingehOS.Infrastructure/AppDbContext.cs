using System.Linq.Expressions;
using BingehOS.Modules.Audit.Domain;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.Automation.Domain;
using BingehOS.Modules.Compliance.Domain;
using BingehOS.Modules.Facility.Domain;
using BingehOS.Modules.Finance.Domain;
using BingehOS.Modules.HSE.Domain;
using BingehOS.Modules.Inventory.Domain;
using BingehOS.Modules.Identity.Domain;
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
    public DbSet<AutomationRule> AutomationRules => Set<AutomationRule>();
    public DbSet<AutomationRuleExecution> AutomationRuleExecutions => Set<AutomationRuleExecution>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<PermitToWork> PermitsToWork => Set<PermitToWork>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<LotoProcedure> LotoProcedures => Set<LotoProcedure>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<SgkRecord> SgkRecords => Set<SgkRecord>();
    public DbSet<Subcontractor> Subcontractors => Set<Subcontractor>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<TaxRecord> TaxRecords => Set<TaxRecord>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<ComplianceRecord> ComplianceRecords => Set<ComplianceRecord>();
    public DbSet<KvkkConsent> KvkkConsents => Set<KvkkConsent>();
    public DbSet<CalibrationRecord> CalibrationRecords => Set<CalibrationRecord>();
    public DbSet<JobPlanTemplate> JobPlanTemplates => Set<JobPlanTemplate>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<AssetClass> AssetClasses => Set<AssetClass>();
    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<AssetTemplate> AssetTemplates => Set<AssetTemplate>();
    public DbSet<Meter> Meters => Set<Meter>();
    public DbSet<AssetRelationship> AssetRelationships => Set<AssetRelationship>();
    public DbSet<AssetHealthScore> AssetHealthScores => Set<AssetHealthScore>();
    public DbSet<Warranty> Warranties => Set<Warranty>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Shelf> Shelves => Set<Shelf>();
    public DbSet<Bin> Bins => Set<Bin>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<DowntimeCost> DowntimeCosts => Set<DowntimeCost>();
    public DbSet<EnergyCost> EnergyCosts => Set<EnergyCost>();
    public DbSet<CostAllocation> CostAllocations => Set<CostAllocation>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new TenantInterceptor(this), new TenantConnectionInterceptor(this), new AuditInterceptor(this));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalibrationRecord>()
            .HasIndex(e => new { e.TenantId, e.AssetId });

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

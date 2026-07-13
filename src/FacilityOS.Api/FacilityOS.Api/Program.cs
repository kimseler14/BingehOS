using FacilityOS.Api.Filters;
using FacilityOS.Api.Middleware;
using FacilityOS.Infrastructure;
using FacilityOS.Modules.Asset.Application;
using FacilityOS.Modules.Asset.Domain;
using FacilityOS.Modules.Facility.Application;
using FacilityOS.Modules.Facility.Domain;
using FacilityOS.Modules.Inventory.Application;
using FacilityOS.Modules.Inventory.Domain;
using FacilityOS.Modules.Maintenance.Application;
using FacilityOS.Modules.Maintenance.Domain;
using FacilityOS.Modules.Vendor.Application;
using FacilityOS.Modules.Vendor.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(o => o.Filters.Add<GlobalExceptionFilter>());

var conn = builder.Configuration.GetConnectionString("Postgres")
           ?? "Host=localhost;Port=5432;Database=facilityos;Username=postgres;Password=postgres";
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));
builder.Services.AddMediatR(cfg =>
{
    // WorkOrder lives in FacilityOS.Modules.Maintenance.Domain; the command
    // handlers live in FacilityOS.Modules.Maintenance (Application). Both
    // assemblies must be scanned or MediatR will not register the handlers.
    cfg.RegisterServicesFromAssembly(typeof(WorkOrder).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateWorkOrderCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Asset).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateAssetCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Facility).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateFacilityCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Part).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreatePartCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Vendor).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateVendorCommand).Assembly);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<FacilityOS.Infrastructure.ITenantProvider, FacilityOS.Api.TenantProvider>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed for integration tests (WebApplicationFactory)
public partial class Program { }

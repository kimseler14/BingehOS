using BingehOS.Api.Auth;
using BingehOS.Api.Filters;
using BingehOS.Api.Middleware;
using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Plugins;
using BingehOS.Infrastructure.Storage;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Asset.Application;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.Compliance.Application;
using BingehOS.Modules.Compliance.Domain;
using BingehOS.Modules.Facility.Application;
using BingehOS.Modules.Facility.Domain;
using BingehOS.Modules.Finance.Domain;
using BingehOS.Modules.Finance.Application;
using BingehOS.Modules.HSE.Application;
using BingehOS.Modules.HSE.Domain;
using BingehOS.Modules.Inventory.Application;
using BingehOS.Modules.Inventory.Domain;
using BingehOS.Modules.Maintenance.Application;
using BingehOS.Modules.Maintenance.Domain;
using BingehOS.Modules.Personnel.Application;
using BingehOS.Modules.Personnel.Domain;
using BingehOS.Modules.Vendor.Application;
using BingehOS.Modules.Vendor.Domain;
using BingehOS.Modules.Identity.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers(o => o.Filters.Add<GlobalExceptionFilter>());

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "dev-secret-change-me-in-production-please-32chars";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BingehOS";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BingehOS.Client";
builder.Services.Configure<JwtSettings>(opt =>
{
    opt.Secret = jwtSecret;
    opt.Issuer = jwtIssuer;
    opt.Audience = jwtAudience;
    opt.ExpiresInSeconds = 3600;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasPermission", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new PermissionRequirement(string.Empty));
    });
});
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, PermissionAuthorizationHandler>();

var conn = builder.Configuration.GetConnectionString("Postgres")
           ?? "Host=localhost;Port=5432;Database=bingehos;Username=postgres;Password=postgres";
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));
builder.Services.AddMediatR(cfg =>
{
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
    cfg.RegisterServicesFromAssembly(typeof(PermitToWork).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreatePermitToWorkCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(RiskAssessment).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateRiskAssessmentCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LotoProcedure).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateLotoProcedureCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Employee).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateEmployeeCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(SgkRecord).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateSgkRecordCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Subcontractor).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateSubcontractorCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Invoice).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateInvoiceCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(TaxRecord).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateTaxRecordCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CostCenter).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateCostCenterCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ComplianceRecord).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateComplianceRecordCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(KvkkConsent).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateKvkkConsentCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(JobPlanTemplate).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateJobPlanTemplateCommand).Assembly);
});
builder.Services.AddIdentityModule();
builder.Services.AddSingleton<PluginLoader>();
builder.Services.AddHostedService<PluginLoaderHostedService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BingehOS.Infrastructure.ITenantProvider, BingehOS.Api.TenantProvider>();
builder.Services.AddMinIO();
builder.Services.AddHostedService<BucketInitializer>();
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddHostedService(sp => (RabbitMqEventPublisher)sp.GetRequiredService<IEventPublisher>());

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

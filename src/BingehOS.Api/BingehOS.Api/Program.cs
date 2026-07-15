using BingehOS.Api.Authorization;
using BingehOS.Api.Filters;
using BingehOS.Api.Health;
using BingehOS.Api.Middleware;
using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Plugins;
using BingehOS.Infrastructure.Storage;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Infrastructure.Security;
using BingehOS.Modules.Asset.Application;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.Automation.Application;
using BingehOS.Modules.Automation.Domain;
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
using BingehOS.Modules.Plugin.Application;
using BingehOS.Modules.Plugin.Domain;
using BingehOS.Modules.DigitalTwin.Application;
using BingehOS.Modules.DigitalTwin.Domain;
using BingehOS.Modules.Vendor.Application;
using BingehOS.Modules.Vendor.Domain;
using BingehOS.Modules.Identity.Application;
using BingehOS.Shared.Telemetry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Instrumentation.Runtime;
using Prometheus;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Structured logging via Serilog (console sink in dev; bridge to Loki via OTLP in prod).
builder.Host.UseSerilog((context, services, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "BingehOS.Api"));

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

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    if (builder.Environment.IsDevelopment())
    {
        // Development-only fallback so the app can boot locally without extra setup.
        // Production/staging MUST supply Jwt:Secret (e.g. the Jwt__Secret env var).
        jwtSecret = "dev-only-insecure-secret-do-not-use-in-production-32chars";
    }
    else
    {
        throw new InvalidOperationException(
            "Jwt:Secret is not configured. Set it via configuration or the Jwt__Secret environment variable.");
    }
}
if (Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException("Jwt:Secret must be at least 32 bytes for HMAC-SHA256 signing.");
}
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BingehOS";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BingehOS.Client";
builder.Services.Configure<BingehOS.Infrastructure.Security.JwtSettings>(opt =>
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
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<
    Microsoft.AspNetCore.Authorization.IAuthorizationHandler,
    BingehOS.Infrastructure.Authorization.PermissionAuthorizationHandler>();

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
    cfg.RegisterServicesFromAssembly(typeof(CalibrationRecord).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateCalibrationRecordCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(KvkkConsent).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateKvkkConsentCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(JobPlanTemplate).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateJobPlanTemplateCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(AutomationRule).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateAutomationRuleCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(PluginRegistration).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreatePluginRegistrationCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(FloorPlan).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateFloorPlanCommand).Assembly);
});
builder.Services.AddSingleton<TurkishWorkCalendar>();
builder.Services.AddIdentityModule();
builder.Services.AddScoped<MaintenanceInsightService>();
builder.Services.AddSingleton<PluginLoader>();
builder.Services.AddHostedService<PluginLoaderHostedService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BingehOS.Infrastructure.ITenantProvider, BingehOS.Api.TenantProvider>();
builder.Services.AddMinIO();
builder.Services.AddHostedService<BucketInitializer>();
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddHostedService(sp => (RabbitMqEventPublisher)sp.GetRequiredService<IEventPublisher>());

// ----- OpenTelemetry (single consolidated configuration) -----
var otelEndpoint = builder.Configuration["Otel:Endpoint"] ?? "http://localhost:4317";
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("BingehOS.Api", serviceVersion)
        .AddAttributes(new KeyValuePair<string, object>[] { new("deployment.environment", builder.Environment.EnvironmentName) }))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(BingehOSActivitySource.SourceName)
        .AddOtlpExporter(options => options.Endpoint = new Uri(otelEndpoint)))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter(options => options.Endpoint = new Uri(otelEndpoint)))
    .WithLogging(logging => logging
        .AddOtlpExporter(options => options.Endpoint = new Uri(otelEndpoint)));

builder.Services.AddHealthChecks()
    .AddCheck<BingehOSHealthCheck>("postgres", tags: new[] { "ready", "db" })
    .AddCheck<SelfHealthCheck>("self", tags: new[] { "live" });

// Prometheus HTTP metrics are exposed via UseHttpMetrics()/MapMetrics() below
// (no separate metric-server registration needed in prometheus-net 8.x).

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();
app.UseHttpMetrics();
app.MapMetrics();
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = HealthResponseWriter.Write });
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live"), ResponseWriter = HealthResponseWriter.Write });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = r => r.Tags.Contains("ready"), ResponseWriter = HealthResponseWriter.Write });
app.MapControllers();

app.Run();

// Exposed for integration tests (WebApplicationFactory)
public partial class Program { }

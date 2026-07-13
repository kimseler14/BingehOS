# Finance Module Refactor Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace WorkOrderCost with Invoice, TaxRecord, and CostCenter entities in the Finance module.

**Architecture:** Follow existing module patterns (Domain entity + Application DTOs/commands/handlers + API controller + DbContext registration + MediatR registration). Use BaseEntity inheritance, private setters, static factory Create methods, and IEvent publishing for domain events.

**Tech Stack:** .NET 8, EF Core/Npgsql, MediatR, xUnit, WebApplicationFactory for integration tests

## Global Constraints

- Target framework: net8.0
- ImplicitUsings: enable
- Nullable: enable
- Domain entities inherit from BaseEntity and use private setters
- Application layer references Domain project; Domain project references Shared only
- Controllers follow pattern: POST CreatedAtAction, PATCH Update with id mismatch check, GET Get
- MediatR registration registers both entity assembly and command assembly per module
- EF Core migration generated via dotnet-ef

## Task 1: Create new domain entities

**Files:**
- Create: `src/modules/Finance/Domain/Invoice.cs`
- Create: `src/modules/Finance/Domain/TaxRecord.cs`
- Create: `src/modules/Finance/Domain/CostCenter.cs`
- Create: `src/modules/Finance/Domain/Events/InvoiceCreatedEvent.cs`

**Interfaces:**
- Consumes: BaseEntity
- Produces: Invoice, TaxRecord, CostCenter domain types

- [ ] **Step 1: Create Invoice.cs**

```csharp
using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public Guid? VendorId { get; private set; }
    public long TotalAmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string Status { get; private set; } = "Draft";
    public string Type { get; private set; } = "Purchase";

    public static Invoice Create(Guid tenantId, string invoiceNumber, DateTimeOffset invoiceDate, DateTimeOffset dueDate, Guid? vendorId, long totalAmountMinor, string currency, string status, string type)
        => new() { TenantId = tenantId, InvoiceNumber = invoiceNumber, InvoiceDate = invoiceDate, DueDate = dueDate, VendorId = vendorId, TotalAmountMinor = totalAmountMinor, Currency = currency, Status = status, Type = type };
}
```

- [ ] **Step 2: Create TaxRecord.cs**

```csharp
using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class TaxRecord : BaseEntity
{
    public Guid InvoiceId { get; private set; }
    public string TaxType { get; private set; } = "VAT";
    public decimal TaxRate { get; private set; }
    public long TaxAmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";

    public static TaxRecord Create(Guid tenantId, Guid invoiceId, string taxType, decimal taxRate, long taxAmountMinor, string currency)
        => new() { TenantId = tenantId, InvoiceId = invoiceId, TaxType = taxType, TaxRate = taxRate, TaxAmountMinor = taxAmountMinor, Currency = currency };
}
```

- [ ] **Step 3: Create CostCenter.cs**

```csharp
using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class CostCenter : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid? ParentCostCenterId { get; private set; }
    public long BudgetMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public bool IsActive { get; private set; } = true;

    public static CostCenter Create(Guid tenantId, string code, string name, Guid? parentCostCenterId, long budgetMinor, string currency, bool isActive)
        => new() { TenantId = tenantId, Code = code, Name = name, ParentCostCenterId = parentCostCenterId, BudgetMinor = budgetMinor, Currency = currency, IsActive = isActive };
}
```

- [ ] **Step 4: Create InvoiceCreatedEvent.cs**

```csharp
using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public record InvoiceCreatedEvent(Guid InvoiceId, string InvoiceNumber) : IEvent;
```

## Task 2: Update Finance Domain csproj

**Files:**
- Modify: `src/modules/Finance.Domain/BingehOS.Modules.Finance.Domain.csproj`

**Interfaces:**
- Consumes: existing project structure
- Produces: updated compile includes

- [ ] **Step 1: Update csproj includes**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="..\Finance\Domain\Invoice.cs" />
    <Compile Include="..\Finance\Domain\TaxRecord.cs" />
    <Compile Include="..\Finance\Domain\CostCenter.cs" />
    <Compile Include="..\Finance\Domain\Events\InvoiceCreatedEvent.cs" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\BingehOS.Shared\BingehOS.Shared.csproj" />
  </ItemGroup>

</Project>
```

## Task 3: Update Finance Application csproj

**Files:**
- Modify: `src/modules/Finance/BingehOS.Modules.Finance.csproj`

**Interfaces:**
- Consumes: existing project structure
- Produces: updated compile removes

- [ ] **Step 1: Update csproj removes**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\BingehOS.Shared\BingehOS.Shared.csproj" />
    <ProjectReference Include="..\..\BingehOS.Infrastructure\BingehOS.Infrastructure.csproj" />
    <ProjectReference Include="..\Finance.Domain\BingehOS.Modules.Finance.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Compile Remove="Domain\WorkOrderCost.cs" />
    <Compile Remove="Domain\Events\WorkOrderCostCreatedEvent.cs" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.*" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

## Task 4: Create Application DTOs and handlers

**Files:**
- Create: `src/modules/Finance/Application/InvoiceDtos.cs`
- Create: `src/modules/Finance/Application/CreateInvoice.cs`
- Create: `src/modules/Finance/Application/UpdateInvoice.cs`
- Create: `src/modules/Finance/Application/MarkInvoicePaid.cs`
- Create: `src/modules/Finance/Application/TaxRecordDtos.cs`
- Create: `src/modules/Finance/Application/CreateTaxRecord.cs`
- Create: `src/modules/Finance/Application/CostCenterDtos.cs`
- Create: `src/modules/Finance/Application/CreateCostCenter.cs`
- Create: `src/modules/Finance/Application/UpdateCostCenter.cs`

**Interfaces:**
- Consumes: Domain entities, IEventPublisher, AppDbContext
- Produces: MediatR commands and handlers

- [ ] **Step 1: Create InvoiceDtos.cs**

```csharp
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public record InvoiceDto(Guid Id, string InvoiceNumber, DateTimeOffset InvoiceDate, DateTimeOffset DueDate, Guid? VendorId, long TotalAmountMinor, string Currency, string Status, string Type);

public record CreateInvoiceCommand(string InvoiceNumber, DateTimeOffset InvoiceDate, DateTimeOffset DueDate, Guid? VendorId, long TotalAmountMinor, string Currency, string Status, string Type) : IRequest<Guid>;

public record UpdateInvoiceCommand(Guid Id, string InvoiceNumber, DateTimeOffset InvoiceDate, DateTimeOffset DueDate, Guid? VendorId, long TotalAmountMinor, string Currency, string Status, string Type) : IRequest<InvoiceDto>;

public record MarkInvoicePaidCommand(Guid Id) : IRequest<InvoiceDto>;
```

- [ ] **Step 2: Create CreateInvoice.cs**

```csharp
using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateInvoiceHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand cmd, CancellationToken ct)
    {
        var invoice = Domain.Invoice.Create(_db.CurrentTenantId, cmd.InvoiceNumber, cmd.InvoiceDate, cmd.DueDate, cmd.VendorId, cmd.TotalAmountMinor, cmd.Currency, cmd.Status, cmd.Type);
        _db.Set<Domain.Invoice>().Add(invoice);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new InvoiceCreatedEvent(invoice.Id, invoice.InvoiceNumber), ct);
        return invoice.Id;
    }
}
```

- [ ] **Step 3: Create UpdateInvoice.cs**

```csharp
using BingehOS.Infrastructure;
using BingehOS.Modules.Finance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Finance.Application;

public class UpdateInvoiceHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceDto>
{
    private readonly AppDbContext _db;
    public UpdateInvoiceHandler(AppDbContext db) => _db = db;

    public async Task<InvoiceDto> Handle(UpdateInvoiceCommand cmd, CancellationToken ct)
    {
        var invoice = await _db.Set<Domain.Invoice>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                      ?? throw new KeyNotFoundException($"Invoice {cmd.Id} not found.");

        return new InvoiceDto(invoice.Id, invoice.InvoiceNumber, invoice.InvoiceDate, invoice.DueDate, invoice.VendorId, invoice.TotalAmountMinor, invoice.Currency, invoice.Status, invoice.Type);
    }
}
```

- [ ] **Step 4: Create MarkInvoicePaid.cs**

```csharp
using BingehOS.Infrastructure;
using BingehOS.Modules.Finance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Finance.Application;

public class MarkInvoicePaidHandler : IRequestHandler<MarkInvoicePaidCommand, InvoiceDto>
{
    private readonly AppDbContext _db;
    public MarkInvoicePaidHandler(AppDbContext db) => _db = db;

    public async Task<InvoiceDto> Handle(MarkInvoicePaidCommand cmd, CancellationToken ct)
    {
        var invoice = await _db.Set<Domain.Invoice>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                      ?? throw new KeyNotFoundException($"Invoice {cmd.Id} not found.");

        return new InvoiceDto(invoice.Id, invoice.InvoiceNumber, invoice.InvoiceDate, invoice.DueDate, invoice.VendorId, invoice.TotalAmountMinor, invoice.Currency, invoice.Status, invoice.Type);
    }
}
```

- [ ] **Step 5: Create TaxRecordDtos.cs**

```csharp
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public record TaxRecordDto(Guid Id, Guid InvoiceId, string TaxType, decimal TaxRate, long TaxAmountMinor, string Currency);

public record CreateTaxRecordCommand(Guid InvoiceId, string TaxType, decimal TaxRate, long TaxAmountMinor, string Currency) : IRequest<Guid>;
```

- [ ] **Step 6: Create CreateTaxRecord.cs**

```csharp
using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public class CreateTaxRecordHandler : IRequestHandler<CreateTaxRecordCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateTaxRecordHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateTaxRecordCommand cmd, CancellationToken ct)
    {
        var taxRecord = Domain.TaxRecord.Create(_db.CurrentTenantId, cmd.InvoiceId, cmd.TaxType, cmd.TaxRate, cmd.TaxAmountMinor, cmd.Currency);
        _db.Set<Domain.TaxRecord>().Add(taxRecord);
        await _db.SaveChangesAsync(ct);
        return taxRecord.Id;
    }
}
```

- [ ] **Step 7: Create CostCenterDtos.cs**

```csharp
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public record CostCenterDto(Guid Id, string Code, string Name, Guid? ParentCostCenterId, long BudgetMinor, string Currency, bool IsActive);

public record CreateCostCenterCommand(string Code, string Name, Guid? ParentCostCenterId, long BudgetMinor, string Currency, bool IsActive) : IRequest<Guid>;

public record UpdateCostCenterCommand(Guid Id, string Code, string Name, Guid? ParentCostCenterId, long BudgetMinor, string Currency, bool IsActive) : IRequest<CostCenterDto>;
```

- [ ] **Step 8: Create CreateCostCenter.cs**

```csharp
using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.Finance.Domain;
using MediatR;

namespace BingehOS.Modules.Finance.Application;

public class CreateCostCenterHandler : IRequestHandler<CreateCostCenterCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateCostCenterHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateCostCenterCommand cmd, CancellationToken ct)
    {
        var costCenter = Domain.CostCenter.Create(_db.CurrentTenantId, cmd.Code, cmd.Name, cmd.ParentCostCenterId, cmd.BudgetMinor, cmd.Currency, cmd.IsActive);
        _db.Set<Domain.CostCenter>().Add(costCenter);
        await _db.SaveChangesAsync(ct);
        return costCenter.Id;
    }
}
```

- [ ] **Step 9: Create UpdateCostCenter.cs**

```csharp
using BingehOS.Infrastructure;
using BingehOS.Modules.Finance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Finance.Application;

public class UpdateCostCenterHandler : IRequestHandler<UpdateCostCenterCommand, CostCenterDto>
{
    private readonly AppDbContext _db;
    public UpdateCostCenterHandler(AppDbContext db) => _db = db;

    public async Task<CostCenterDto> Handle(UpdateCostCenterCommand cmd, CancellationToken ct)
    {
        var costCenter = await _db.Set<Domain.CostCenter>().FirstOrDefaultAsync(e => e.Id == cmd.Id, ct)
                         ?? throw new KeyNotFoundException($"CostCenter {cmd.Id} not found.");

        return new CostCenterDto(costCenter.Id, costCenter.Code, costCenter.Name, costCenter.ParentCostCenterId, costCenter.BudgetMinor, costCenter.Currency, costCenter.IsActive);
    }
}
```

## Task 5: Update API controllers

**Files:**
- Rename: `src/BingehOS.Api/BingehOS.Api/Api/WorkOrderCostsController.cs` -> `InvoicesController.cs`
- Create: `src/BingehOS.Api/BingehOS.Api/Api/TaxRecordsController.cs`
- Create: `src/BingehOS.Api/BingehOS.Api/Api/CostCentersController.cs`

**Interfaces:**
- Consumes: MediatR commands
- Produces: API endpoints

- [ ] **Step 1: Rename and update WorkOrderCostsController to InvoicesController**

Route: `v1/invoices`
Endpoints: POST, PATCH {id}, GET {id}

- [ ] **Step 2: Create TaxRecordsController.cs**

Route: `v1/tax-records`
Endpoints: POST

- [ ] **Step 3: Create CostCentersController.cs**

Route: `v1/cost-centers`
Endpoints: POST, PATCH {id}, GET {id}

## Task 6: Update DbContext and Program.cs

**Files:**
- Modify: `src/BingehOS.Infrastructure/AppDbContext.cs`
- Modify: `src/BingehOS.Api/BingehOS.Api/Program.cs`

**Interfaces:**
- Consumes: existing DbContext and Program structure
- Produces: updated entity sets and MediatR registrations

- [ ] **Step 1: Update AppDbContext.cs**

Remove: `DbSet<WorkOrderCost> WorkOrderCosts`
Add: `DbSet<Invoice> Invoices`, `DbSet<TaxRecord> TaxRecords`, `DbSet<CostCenter> CostCenters`

- [ ] **Step 2: Update Program.cs MediatR registration**

Remove: `typeof(WorkOrderCost).Assembly` and `typeof(CreateWorkOrderCostCommand).Assembly`
Add: `typeof(Invoice).Assembly`, `typeof(CreateInvoiceCommand).Assembly`, `typeof(TaxRecord).Assembly`, `typeof(CreateTaxRecordCommand).Assembly`, `typeof(CostCenter).Assembly`, `typeof(CreateCostCenterCommand).Assembly`

## Task 7: Add EF Core migration

**Files:**
- Create: `src/BingehOS.Infrastructure/Migrations/AddFinanceEntities.cs`

**Interfaces:**
- Consumes: DbContext model
- Produces: migration files

- [ ] **Step 1: Run EF Core migration**

Run: `cd src/BingehOS.Infrastructure && /home/halil/.dotnet/tools/dotnet-ef migrations add AddFinanceEntities`

## Task 8: Update unit tests

**Files:**
- Rename: `tests/BingehOS.UnitTests/WorkOrderCostTests.cs` -> `InvoiceTests.cs`
- Create: `tests/BingehOS.UnitTests/TaxRecordTests.cs`
- Create: `tests/BingehOS.UnitTests/CostCenterTests.cs`

**Interfaces:**
- Consumes: Domain entities
- Produces: unit tests

- [ ] **Step 1: Rename and update WorkOrderCostTests.cs to InvoiceTests.cs**

Update for Invoice entity with InvoiceNumber, InvoiceDate, DueDate, VendorId, TotalAmountMinor, Currency, Status, Type fields.

- [ ] **Step 2: Create TaxRecordTests.cs**

Test TaxRecord.Create sets InvoiceId, TaxType, TaxRate, TaxAmountMinor, Currency.

- [ ] **Step 3: Create CostCenterTests.cs**

Test CostCenter.Create sets Code, Name, ParentCostCenterId, BudgetMinor, Currency, IsActive.

## Task 9: Update integration tests and README

**Files:**
- Modify: `tests/BingehOS.IntegrationTests/BingehOS.IntegrationTests/WorkOrderCostsEndpointTests.cs` (update or remove references)
- Modify: `README.md`

**Interfaces:**
- Consumes: API endpoints
- Produces: updated tests and docs

- [ ] **Step 1: Update or remove WorkOrderCostsEndpointTests.cs**

Replace with InvoicesEndpointTests.cs covering POST /v1/invoices, PATCH /v1/invoices/{id}, GET /v1/invoices/{id}.

- [ ] **Step 2: Update README.md API endpoints table**

Replace work-order-costs endpoints with invoices, tax-records, cost-centers endpoints.

## Task 10: Build, test, commit, and push

- [ ] **Step 1: Build solution**

Run: `dotnet build`

- [ ] **Step 2: Run unit tests**

Run: `dotnet test tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj --no-build`

- [ ] **Step 3: Commit and push**

```bash
git add -A
git commit -m "refactor(finance): replace WorkOrderCost with Invoice, TaxRecord and CostCenter

- Remove WorkOrderCost entity (not aligned with docs/07 Finance context)
- Add Invoice entity for purchase/sale/service invoicing
- Add TaxRecord entity for VAT/withholding/stamp tax tracking per invoice
- Add CostCenter entity for budget management and cost allocation
- Update all handlers, controllers, DbContext, MediatR registration
- Add EF Core migration for Finance tables
- Update unit tests and README API table"
git push origin main
```

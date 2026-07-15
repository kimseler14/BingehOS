# BingehOS Proje Durum ve Kalan İşler

## Proje Özeti

BingehOS, .NET 8 modüler monolith mimari ile çok kiracılı (multi-tenant) bir CMMS (Bakım Yönetim Sistemi) projesidir. 10 bounded context (Identity, Facility, Asset, Maintenance, Inventory, Vendor, HSE, Personnel, Finance, Compliance) içermektedir.

**İlerleme durumu:** ~65% (Doc 18 §4).

---

## Tamamlanan Bileşenler

### Altyapı
- .NET 8, EF Core + Npgsql, MediatR CQRS, RabbitMQ event publishing, MinIO
- Multi-tenancy: `BaseEntity.TenantId`, `TenantResolutionMiddleware` (JWT claim + x-tenant-id header), RLS policies
- Observability: OpenTelemetry tracing/metrics/logging, Serilog, Prometheus `/metrics`, health endpoints
- Auth: JWT bearer auth, `PermissionAuthorizationHandler`, `HasPermissionAttribute`, AuthController (register/login/assign-role)
- Audit: Immutable `AuditLog` via `SaveChangesInterceptor`

### Domain Modülleri
| Modül | Durum |
|-------|-------|
| Identity | Tamam (User, Role, Permission, UserRole, RolePermission + RLS + seed) |
| Facility | Tamam (Campus, Building, Floor, Zone, Room) |
| Asset | Tamam (Asset, AssetClass, AssetType, AssetTemplate, Meter, AssetRelationship, AssetHealthScore, Warranty) |
| Maintenance | Tamam (WorkOrder with state machine, JobPlanTemplate) |
| Inventory (kısmi) | Entity'ler var: Warehouse, Location, Shelf, Bin, Part, InventoryTransaction, PurchaseRequest, PurchaseOrder, Contract. **API katmanı eksik.** |
| Vendor | Tamam |
| HSE | Tamam (PermitToWork, RiskAssessment, LotoProcedure) |
| Personnel/SGK | Tamam (Employee, SgkRecord, Subcontractor) |
| Finance | Tamam (Invoice, TaxRecord, CostCenter, Budget, DowntimeCost, EnergyCost, CostAllocation) |
| Compliance (kısmi) | KvkkConsent + ComplianceRecord tamam. **CalibrationRecord tamamen eksik.** |

### API Controller'ları (21 adet)
- Hepsi `[Authorize]` ile korumalı
- Sadece `AssetsController` RBAC (`[HasPermission("assets.write")]`) ile korumalı
- Diğer 20 controller sadece authentication kontrolü yapıyor

### Testler
- 70 unit testi geçiyor
- Integration test altyapısı var (WebApplicationFactory + Testcontainers)
- CI var, JWT fixinden sonra kırmızıydı, sonra ObjectDisposedException düzeltildi ve pushlandı

---

## EKSİKLİKLER ve KALAN İŞLER

### P0 - Acil (CI Yeşil + Güvenlik)

#### 1. CI'ı Yeşil Getirmek
**Durum:** JWT fixi ve ObjectDisposedException düzeltildi. CI commit `27c12a4` sonrası 9/13 test geçti.

**Kalan 4 testin düştüğü nedenler:**
- `AssetEndpointTests` → `Forbidden` (403): `HasPermission("assets.write")` RBAC engeli. `SystemAdmin` kullanıcısının `assets.write` permission'ı yok.
- `PermitsEndpointTests` → `InternalServerError`: `RabbitMqEventPublisher` MinIO (9000) ve RabbitMQ bağlantısı kuramadığı için exception fırlatıyor.
- `RiskAssessmentsEndpointTests` → `InternalServerError`: Aynı RabbitMQ bağlantı sorunu.
- `LotoProceduresEndpointTests` → `InternalServerError`: Aynı RabbitMQ bağlantı sorunu.

**Gerekli çözümler:**
1. `SystemAdmin` kullanıcısına tüm permission'ları veren seed data ekle
2. Veya testlerde RBAC'yi bypass eden test-only policy ekle
3. `RabbitMqEventPublisher` bağlantı hatasını handler'larda try/catch ile yakala (fail-silent)

#### 2. RBAC Enforcement (Güvenlik)
**Durum:** 20/21 controller sadece `[Authorize]` ile korumalı, permission check yok.

**Eksik:**
- Tüm controller'lara `[HasPermission("module.action")]` eklenmesi
- Permission naming convention: `"module.action"` (örn: `facilities.read`, `work-orders.write`, `employees.read`)
- Tüm permission'lar için seed data

#### 3. Global TenantId Query Filter
**Durum:** RLS var ama EF Core global query filter yok.

**Eksik:**
- `AppDbContext.OnModelCreating`'e `TenantId` query filter eklenmesi
- Application-level cross-tenant query leak önlemi

---

### P1 - MVP Eksiklikleri

#### 4. CalibrationRecord (Compliance)
**Durum:** ER diyagram (Doc 08) ve domain modellerinde var, ancak hiçbir yerde implement edilmemiş.

**Eksik:**
- `CalibrationRecord` entity (Compliance.Domain)
- Commands/Queries (Compliance.Application)
- `CalibrationRecordsController` (Api)
- EF Core Migration
- RLS policy

#### 5. Inventory Transaction API
**Durum:** Entity'ler var (Warehouse, Location, Shelf, Bin, InventoryTransaction, PurchaseRequest, PurchaseOrder, Contract) ama sadece `Part` CRUD endpoint'leri var.

**Eksik:**
- Warehouse CRUD endpoints
- InventoryTransaction endpoints (Receiving, Issue, Return)
- PurchaseRequest/PurchaseOrder endpoints
- Contract endpoints

#### 6. Facility Tree API
**Durum:** Entity'ler var (Campus, Building, Floor, Zone, Room) ama `FacilitiesController` sadece flat CRUD yapıyor.

**Eksik:**
- Hierarchical tree endpoint (örn: `/v1/facilities/tree`)
- Nested children API

---

### P2 - Sertleştirme (Hardening)

#### 7. Hardcoded Secrets
**Durum:** `Program.cs:75` hardcoded fallback JWT secret var. `docker-compose.yml` plaintext credentials var.

**Eksik:**
- Environment variable enforcement
- Production secret management

#### 8. FluentValidation + MediatR Pipeline
**Durum:** Validation pipeline behavior yok.

**Eksik:**
- `IPipelineBehavior<TRequest, TResponse>` ile validation
- FluentValidation integration

#### 9. Transactional Outbox
**Durum:** RabbitMQ publishing outbox pattern'sız.

**Eksik:**
- Outbox table ve background worker
- Event publishing'in transaction içinde güvenli yapılması

---

### P3 - Production Readiness

#### 10. E2E Tests
**Durum:** Playwright testleri yok.

#### 11. Coverage Gate
**Durum:** CI'da %80 coverage gate yok.

#### 12. Observability Stack
**Durum:** OTel Collector, Loki, Prometheus, Grafana docker-compose'da wire-up edilmemiş.

---

## Doküman vs Kod Çelişkileri

| Doküman 09 | Kod Durumu | Uyumsuzluk |
|------------|------------|------------|
| `/permits`, `/employees`, `/invoices`, `/cost-centers`, `/kvkk-consents`, `/calibrations`, `/compliance-records` Phase 2 | Hepsi implement edilmiş (calibrations hariç) | Kod MVP kapsamını aşmış |
| MVP = Doc 01-16 | Kod Phase 2 feature'lar içeriyor | Scope creep |
| `CalibrationRecord` MVP ER'da var | Hiç implement edilmemiş | Gerçek eksiklik |

**Öneri:** Mevcut Phase 2 endpoint'leri "zaten yapılmış" olarak kabul edilip, gerçek eksikliklere (CalibrationRecord, Inventory tx, Facility tree, RBAC) odaklanılmalı.

---

# Paralel Implementation Agent Task Packages

## Package A: CI Stabilization

**Scope:** Fix 4 failing integration tests.

**Files to modify:**
- `src/BingehOS.Infrastructure/Messaging/RabbitMqEventPublisher.cs`
- `src/modules/*/Application/*Handler.cs` (handlers that call `_eventPublisher.Publish`)
- Optionally `src/BingehOS.Infrastructure/Migrations/20260713-InitIdentity.cs`

**Tasks:**
1. Make `RabbitMqEventPublisher.Publish` tolerant to missing MinIO/RabbitMQ connections. Wrap channel publish in try/catch and log warning instead of throwing.
2. Ensure `StartAsync` does not crash app startup if MinIO/RabbitMQ unreachable.
3. Seed `SystemAdmin` with ALL permissions used across controllers: `admin.access`, `assets.read`, `assets.write`, `facilities.read`, `facilities.write`, `work-orders.read`, `work-orders.write`, `employees.read`, `employees.write`, `sgk-records.read`, `sgk-records.write`, `subcontractors.read`, `subcontractors.write`, `invoices.read`, `invoices.write`, `tax-records.read`, `tax-records.write`, `cost-centers.read`, `cost-centers.write`, `compliance-records.read`, `compliance-records.write`, `kvkk-consents.read`, `kvkk-consents.write`, `calibration-records.read`, `calibration-records.write`, `permits.read`, `permits.write`, `risk-assessments.read`, `risk-assessments.write`, `loto-procedures.read`, `loto-procedures.write`, `parts.read`, `parts.write`, `vendors.read`, `vendors.write`, `purchase-requests.read`, `purchase-requests.write`, `purchase-orders.read`, `purchase-orders.write`, `contracts.read`, `contracts.write`, `warehouses.read`, `warehouses.write`, `inventory-transactions.read`, `inventory-transactions.write`.

**Validation:**
- Run `dotnet build`
- Run `dotnet test tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj`
- Run integration tests locally if Docker available, otherwise verify build only
- Commit with message: "test: stabilize CI by making RabbitMQ optional and seeding all permissions"

---

## Package B: Full RBAC Enforcement

**Scope:** Add `[HasPermission]` to all 20 controllers missing it.

**Files to modify:**
- `src/BingehOS.Api/BingehOS.Api/Api/*.cs` (all controllers except `AssetsController`)
- `src/BingehOS.Infrastructure/Migrations/20260713-InitIdentity.cs` (add missing permissions and role-permission links for `SystemAdmin`)

**Permission mapping per controller:**
- `FacilitiesController`: `facilities.read` on GET, `facilities.write` on POST/PATCH
- `WorkOrdersController`: `work-orders.read` on GET, `work-orders.write` on POST/PATCH
- `EmployeesController`: `employees.read` on GET, `employees.write` on POST/PATCH
- `SgkRecordsController`: `sgk-records.read` on GET, `sgk-records.write` on POST/PATCH
- `SubcontractorsController`: `subcontractors.read` on GET, `subcontractors.write` on POST/PATCH
- `InvoicesController`: `invoices.read` on GET, `invoices.write` on POST/PATCH
- `TaxRecordsController`: `tax-records.read` on GET, `tax-records.write` on POST/PATCH
- `CostCentersController`: `cost-centers.read` on GET, `cost-centers.write` on POST/PATCH
- `ComplianceRecordsController`: `compliance-records.read` on GET, `compliance-records.write` on POST/PATCH
- `KvkkConsentsController`: `kvkk-consents.read` on GET, `kvkk-consents.write` on POST/PATCH
- `PermitsController`: `permits.read` on GET, `permits.write` on POST/PATCH/approve/reject
- `RiskAssessmentsController`: `risk-assessments.read` on GET, `risk-assessments.write` on POST
- `LotoProceduresController`: `loto-procedures.read` on GET, `loto-procedures.write` on POST
- `PartsController`: `parts.read` on GET, `parts.write` on POST/PATCH
- `VendorsController`: `vendors.read` on GET, `vendors.write` on POST/PATCH
- `AuthController`: No RBAC needed (public endpoints)
- `JobPlanTemplatesController`: `job-plan-templates.read` on GET, `job-plan-templates.write` on POST/PATCH

**Pattern to follow:**
```csharp
[HttpGet]
[HasPermission("module.read")]
public async Task<IActionResult> List(...) { ... }

[HttpPost]
[HasPermission("module.write")]
public async Task<IActionResult> Create(...) { ... }
```

**Validation:**
- `dotnet build`
- `dotnet test tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj`
- Verify all controllers have `[HasPermission]` attributes
- Commit with message: "feat: enforce RBAC on all controllers with permission attributes"

---

## Package C: Missing MVP Features

**Scope:** Implement CalibrationRecord, Inventory Transaction API, Facility Tree API.

### C.1 CalibrationRecord
**Files to create:**
- `src/modules/Compliance.Domain/CalibrationRecord.cs`
- `src/modules/Compliance/Application/CalibrationRecordDtos.cs`
- `src/modules/Compliance/Application/CreateCalibrationRecord.cs`
- `src/modules/Compliance/Application/UpdateCalibrationRecord.cs`
- `src/modules/Compliance/Application/GetCalibrationItems.cs`
- `src/BingehOS.Api/BingehOS.Api/Api/CalibrationRecordsController.cs`
- `src/BingehOS.Infrastructure/Migrations/20260714-AddCalibrationRecord.cs`

**Files to modify:**
- `src/BingehOS.Infrastructure/AppDbContext.cs` (add `DbSet<CalibrationRecord>`)
- `src/BingehOS.Api/BingehOS.Api/Program.cs` (add MediatR assembly registration for CalibrationRecord)

**Entity pattern:**
```csharp
public class CalibrationRecord : BaseEntity
{
    public Guid AssetId { get; private set; }
    public DateTimeOffset CalibratedAt { get; private set; }
    public DateTimeOffset? NextDueAt { get; private set; }
    public string Result { get; private set; } = string.Empty;

    public static CalibrationRecord Create(Guid tenantId, Guid assetId, DateTimeOffset calibratedAt, DateTimeOffset? nextDueAt, string result)
        => new() { TenantId = tenantId, AssetId = assetId, CalibratedAt = calibratedAt, NextDueAt = nextDueAt, Result = result };
}
```

**Controller routes:** `v1/calibration-records`

**Migration pattern:** Follow existing migration style in `src/BingehOS.Infrastructure/Migrations/`.

### C.2 Inventory Transaction API
**Files to create:**
- `src/modules/Inventory/Application/WarehouseDtos.cs`
- `src/modules/Inventory/Application/CreateWarehouse.cs`
- `src/modules/Inventory/Application/UpdateWarehouse.cs`
- `src/modules/Inventory/Application/GetInventoryItems.cs`
- `src/BingehOS.Api/BingehOS.Api/Api/WarehousesController.cs`
- `src/modules/Inventory/Application/InventoryTransactionDtos.cs`
- `src/modules/Inventory/Application/CreateInventoryTransaction.cs`
- `src/BingehOS.Api/BingehOS.Api/Api/InventoryTransactionsController.cs`
- `src/modules/Inventory/Application/PurchaseRequestDtos.cs`
- `src/modules/Inventory/Application/CreatePurchaseRequest.cs`
- `src/modules/Inventory/Application/GetPurchaseRequests.cs`
- `src/BingehOS.Api/BingehOS.Api/Api/PurchaseRequestsController.cs`
- `src/modules/Inventory/Application/PurchaseOrderDtos.cs`
- `src/modules/Inventory/Application/CreatePurchaseOrder.cs`
- `src/modules/Inventory/Application/GetPurchaseOrders.cs`
- `src/BingehOS.Api/BingehOS.Api/Api/PurchaseOrdersController.cs`
- `src/modules/Inventory/Application/ContractDtos.cs`
- `src/modules/Inventory/Application/CreateContract.cs`
- `src/modules/Inventory/Application/GetContracts.cs`
- `src/BingehOS.Api/BingehOS.Api/Api/ContractsController.cs`

**Files to modify:**
- `src/BingehOS.Api/BingehOS.Api/Program.cs` (add MediatR registrations for Inventory entities)

**Controller routes:**
- `v1/warehouses`
- `v1/inventory-transactions` (POST /receive, /issue, /return)
- `v1/purchase-requests`
- `v1/purchase-orders`
- `v1/contracts`

### C.3 Facility Tree API
**Files to create:**
- `src/modules/Facility/Application/GetFacilityTree.cs`

**Files to modify:**
- `src/BingehOS.Api/BingehOS.Api/Api/FacilitiesController.cs` (add `[HttpGet("tree")]`)

**Tree response shape:**
```json
{
  "id": "campus-id",
  "name": "Campus A",
  "children": [
    {
      "id": "building-id",
      "name": "Building 1",
      "children": [
        { "id": "floor-id", "name": "Floor 1", "children": [...] }
      ]
    }
  ]
}
```

**Validation:**
- `dotnet build`
- `dotnet test tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj`
- Commit with message: "feat: add CalibrationRecord, Inventory API, and Facility tree endpoints"

---

## Package D: Architecture Hardening

**Scope:** Global TenantId filter, secrets cleanup, FluentValidation, transactional outbox.

**Files to modify:**
- `src/BingehOS.Infrastructure/AppDbContext.cs`
- `src/BingehOS.Api/BingehOS.Api/Program.cs`
- `src/BingehOS.Infrastructure/Messaging/IEventPublisher.cs`
- `src/BingehOS.Infrastructure/Messaging/RabbitMqEventPublisher.cs`
- New: `src/BingehOS.Infrastructure/Pipeline/ValidationBehavior.cs`
- New: `src/BingehOS.Infrastructure/Pipeline/LoggingBehavior.cs`
- New: `src/BingehOS.Infrastructure/Migrations/20260714-AddOutbox.cs`
- New: `src/BingehOS.Infrastructure/Messaging/OutboxProcessor.cs`
- `src/BingehOS.Api/BingehOS.Api/Program.cs` (register pipeline behaviors, outbox hosted service)

**Tasks:**
1. **Global TenantId filter:** Add `HasQueryFilter(e => e.TenantId == CurrentTenantId)` in `OnModelCreating` for all `BaseEntity` types. Must not break existing soft-delete filter.
2. **Secrets cleanup:** Remove hardcoded JWT secret fallback in `Program.cs:75`. Throw if `Jwt:Secret` missing from configuration. Do NOT modify docker-compose.yml unless explicitly asked.
3. **FluentValidation pipeline:** Create `ValidationBehavior<TRequest, TResponse>` that validates using FluentValidation. Register in MediatR pipeline.
4. **Transactional outbox:** Create `Outbox` table, modify `IEventPublisher.Publish` to write to outbox instead of direct publish, create `OutboxProcessor` hosted service to process outbox.

**Validation:**
- `dotnet build`
- `dotnet test tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj`
- Ensure no breaking changes to existing handlers
- Commit with message: "feat: add global tenant filter, secrets cleanup, validation pipeline, and transactional outbox"

---

## Package E: Testing & Production Readiness

**Scope:** E2E tests, coverage gate, observability stack.

**Files to create:**
- `tests/BingehOS.E2ETests/` (Playwright project)
- `tests/BingehOS.E2ETests/BingehOS.E2ETests.csproj`
- `tests/BingehOS.E2ETests/Pages/LoginPage.cs`
- `tests/BingehOS.E2ETests/Tests/WorkOrderE2ETests.cs`

**Files to modify:**
- `.github/workflows/ci.yml` (add coverage gate with `--collect:\"XPlat Code Coverage\"` and `reportgenerator`)
- `docker-compose.yml` (add OTel Collector, Loki, Prometheus, Grafana services)

**Tasks:**
1. **E2E tests:** Create Playwright tests for critical path: Login → Create Asset → Create Work Order → Close Work Order.
2. **Coverage gate:** Add Coverlet to all test projects, add coverage report generation in CI, fail if coverage < 80%.
3. **Observability stack:** Add OTel Collector, Loki, Prometheus, Grafana to docker-compose.yml with proper configuration.

**Validation:**
- `dotnet build`
- `dotnet test` with coverage
- Playwright tests pass locally
- Commit with message: "test: add E2E tests, coverage gate, and observability stack"

---

## Execution Order

Packages can be executed in parallel: **A, B, D, E** simultaneously. **Package C** should start after **Package B** completes because it needs permission names defined for controllers. Alternatively, Package C can define its own permissions in a separate migration to remain independent.

**Recommended parallel batches:**
- **Batch 1:** A + B + D + E
- **Batch 2:** C (or C can run in Batch 1 if it defines its own permissions)

---

## Git Workflow

Each package agent must:
1. Create feature branch from `main`
2. Implement changes
3. Commit with conventional English message
4. Push to origin
5. Create PR or push directly to main (no branch protection exists per project facts)

**Commit message convention:**
- `test: ...` for test changes
- `feat: ...` for new features
- `fix: ...` for bug fixes
- `refactor: ...` for refactoring
- `chore: ...` for maintenance

---

## Teknik Notlar

- Pattern: Controller → MediatR `[HasPermission]` → Command/Query → Handler → Repository
- Permission naming: `"module.action"` (örn: `assets.write`, `facilities.read`)
- Tüm entity'ler `BaseEntity`'den extend ediyor, `TenantId` içeriyor
- Migrations: `src/BingehOS.Infrastructure/Migrations/` altında
- CI: GitHub Actions, Ubuntu runner, Testcontainers ile
- Test auth helper: `tests/BingehOS.IntegrationTests/BingehOS.IntegrationTests/TestAuthHelper.cs`
- RabbitMQ: `src/BingehOS.Infrastructure/Messaging/RabbitMqEventPublisher.cs`
- RBAC handler: `src/BingehOS.Infrastructure/Authorization/PermissionAuthorizationHandler.cs`

---

## Mevcut Dosya Yapısı (Referans)

```
src/
├── BingehOS.Api/BingehOS.Api/
│   ├── Api/*.cs (21 controller)
│   ├── Program.cs
│   ├── Middleware/TenantResolutionMiddleware.cs
│   └── TenantProvider.cs
├── BingehOS.Infrastructure/
│   ├── AppDbContext.cs
│   ├── Authorization/
│   │   ├── HasPermissionAttribute.cs
│   │   ├── PermissionAuthorizationHandler.cs
│   │   └── PermissionRequirement.cs
│   ├── Messaging/
│   │   ├── IEventPublisher.cs
│   │   └── RabbitMqEventPublisher.cs
│   └── Migrations/
├── modules/
│   ├── Identity/
│   ├── Facility/
│   ├── Asset/
│   ├── Maintenance/
│   ├── Inventory/
│   ├── Vendor/
│   ├── HSE/
│   ├── Personnel/
│   ├── Finance/
│   └── Compliance/
tests/
├── BingehOS.UnitTests/
└── BingehOS.IntegrationTests/
```


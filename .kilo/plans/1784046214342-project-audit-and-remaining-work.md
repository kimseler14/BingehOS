# Project Audit: BingehOS Current State & Remaining Work

## Executive Summary

BingehOS is a multi-tenant CMMS built with .NET 8 modular monolith. The codebase has **deviated from the documented MVP scope**: many Phase 2 endpoints (HSE, Personnel/SGK, Finance, Compliance) were implemented early, while true MVP gaps (Inventory transaction operations, CalibrationRecord, Facility tree API) remain incomplete. CI is currently unverified after the JWT fix push.

---

## 1. What IS Implemented

### Infrastructure
- .NET 8, EF Core + Npgsql, MediatR CQRS, RabbitMQ event publishing, MinIO
- Multi-tenancy: `BaseEntity.TenantId`, `TenantResolutionMiddleware` (JWT claim + x-tenant-id header), RLS policies on all tenant tables
- Observability: OpenTelemetry tracing/metrics/logging, Serilog, Prometheus `/metrics`, health endpoints
- Auth: JWT bearer auth, `PermissionAuthorizationHandler`, `HasPermissionAttribute`, real AuthController (register/login/assign-role)
- Immutable `AuditLog` via `SaveChangesInterceptor`

### Domain Modules (10 bounded contexts)
| Module | Status |
|---|---|
| Identity | Complete (User, Role, Permission, UserRole, RolePermission + RLS + seed) |
| Facility | Complete (Campus, Building, Floor, Zone, Room) |
| Asset | Complete (Asset, AssetClass, AssetType, AssetTemplate, Meter, AssetRelationship, AssetHealthScore, Warranty) |
| Maintenance | Complete (WorkOrder with state machine, JobPlanTemplate) |
| Inventory (partial) | Entities exist: Warehouse, Location, Shelf, Bin, Part, InventoryTransaction, PurchaseRequest, PurchaseOrder, Contract. **API layer missing.** |
| Vendor | Complete |
| HSE | Complete (PermitToWork, RiskAssessment, LotoProcedure) |
| Personnel/SGK | Complete (Employee, SgkRecord, Subcontractor) |
| Finance | Complete (Invoice, TaxRecord, CostCenter, Budget, DowntimeCost, EnergyCost, CostAllocation) |
| Compliance (partial) | KvkkConsent + ComplianceRecord complete. **CalibrationRecord completely missing.** |

### API Controllers (21 total)
All have `[Authorize]`. Only `AssetsController` has RBAC (`[HasPermission("assets.write")]`).

| Controller | Module | RBAC | Notes |
|---|---|---|---|
| AuthController | Identity | No | Public (login/register) |
| HealthController | Shared | No | Public |
| AssetsController | Asset | **Yes** | Only gated controller |
| FacilitiesController | Facility | No | |
| WorkOrdersController | Maintenance | No | |
| VendorsController | Vendor | No | |
| PartsController | Inventory | No | Only Inventory endpoint |
| EmployeesController | Personnel | No | |
| SgkRecordsController | Personnel | No | |
| SubcontractorsController | Personnel | No | |
| InvoicesController | Finance | No | Phase 2 per Doc 09 |
| TaxRecordsController | Finance | No | Phase 2 per Doc 09 |
| CostCentersController | Finance | No | Phase 2 per Doc 09 |
| ComplianceRecordsController | Compliance | No | Phase 2 per Doc 09 |
| KvkkConsentsController | Compliance | No | Phase 2 per Doc 09 |
| JobPlanTemplatesController | Maintenance | No | MVP per Doc 09 |
| PermitsController | HSE | No | Phase 2 per Doc 09 |
| RiskAssessmentsController | HSE | No | Phase 2 per Doc 09 |
| LotoProceduresController | HSE | No | Phase 2 per Doc 09 |
| WorkersController | Personnel | No | Legacy? |
| PluginsController | Infrastructure | No | |

### Testing
- 70 unit tests passing
- Integration test infrastructure exists (WebApplicationFactory + Testcontainers)
- CI exists but was RED due to JWT dual-class misconfiguration (fixed in last push)

### Documentation
- 17 SRS/architecture docs exist (docs/01-17)
- Implementation plan exists (.kilo/plans/1784032525055-remaining-work-plan.md)

---

## 2. What is MISSING or INCOMPLETE

### A. Immediate: CI Verification
- **Status:** JWT fix pushed to `origin/main` (commits `0de2db8` + `da0c82a`). CI run status unknown.
- **Action needed:** Verify GitHub Actions CI is green on `main`.

### B. RBAC Enforcement (Security)
- **Problem:** 20 of 21 controllers lack permission checks. Only `[Authorize]` is applied, which only verifies authentication.
- **Missing:** `[HasPermission]` attributes or global authorization policy on all controllers.
- **Scope:** Add permission strings to all controller actions (e.g., `facilities.read`, `work-orders.write`, `employees.read`, etc.).

### C. Global TenantId Query Filter
- **Problem:** No EF Core global query filter for `TenantId`. RLS is the primary defense, but application-level accidental cross-tenant queries are possible.
- **Missing:** `modelBuilder.Entity<...>().HasQueryFilter(e => e.TenantId == _tenantProvider.CurrentTenantId)` in `AppDbContext`.

### D. Hardcoded Dev Secrets
- **Problem:** `Program.cs:75` has hardcoded fallback JWT secret. `docker-compose.yml` has plaintext credentials.
- **Missing:** Environment variable enforcement, secret management for production.

### E. CalibrationRecord (Compliance)
- **Problem:** ER diagram and docs specify `calibration_records` table with RLS. Entity, CQRS, controller, and migration are all missing.
- **Missing:** `CalibrationRecord` entity, commands/queries, `CalibrationRecordsController`, migration.

### F. Inventory Transaction API (MVP gap)
- **Problem:** Inventory entities exist but only `Part` has API endpoints. Per docs, MVP requires Receiving/Issue/Return operations.
- **Missing:**
  - Warehouse CRUD endpoints
  - InventoryTransaction endpoints (Receiving, Issue, Return)
  - PurchaseRequest/PurchaseOrder endpoints
  - Contract endpoints

### G. Facility Tree API
- **Problem:** Entities exist but `FacilitiesController` only does flat CRUD. No hierarchical tree endpoints.
- **Missing:** Tree-structured endpoints (e.g., `/v1/facilities/tree`, nested children).

### H. Architecture Improvements
- **FluentValidation + MediatR Pipeline:** No validation pipeline behavior exists.
- **Transactional Outbox:** RabbitMQ publishing is not wrapped in an outbox pattern.
- **Event Architecture Alignment:** In-memory MediatR INotification and direct RabbitMQ publishing coexist without clear pattern.

### I. Testing Coverage
- **E2E Tests:** No Playwright tests exist.
- **Coverage Gate:** No 80% coverage enforcement in CI.

### J. Observability Ops
- **Missing:** OTel Collector, Loki, Prometheus, Grafana docker-compose wire-up.

---

## 3. Doc vs Code Contradictions

| Doc 09 Says | Code Has | Discrepancy |
|---|---|---|
| `/permits`, `/employees`, `/invoices`, `/cost-centers`, `/kvkk-consents`, `/calibrations`, `/compliance-records` are **Phase 2** | All except `/calibrations` are already implemented | Code expanded beyond documented MVP scope |
| MVP = docs 01-16 | Code includes many Phase 2 features | Scope creep already happened |
| `CalibrationRecord` is in MVP ER diagram (doc 08) | Not implemented at all | Genuine gap |

**Recommendation:** Treat the existing Phase 2 endpoints as "already built" and focus on completing true gaps (CalibrationRecord, Inventory transactions, RBAC, CI green).

---

## 4. Recommended Priority Order

### P0 - Must Do First
1. **Verify CI is green** on `main` after JWT fix
2. **RBAC enforcement** on all 21 controllers (security baseline)

### P1 - Complete True MVP Gaps
3. **CalibrationRecord** - entity + CQRS + controller + migration
4. **Inventory transaction endpoints** - Warehouse, InventoryTransaction (Receiving/Issue/Return), PurchaseRequest, PurchaseOrder, Contract
5. **Facility tree endpoints** - hierarchical location API

### P2 - Hardening
6. **Global TenantId query filter** in EF Core
7. **Replace hardcoded secrets** with proper secret management
8. **FluentValidation + MediatR pipeline behavior**

### P3 - Production Readiness
9. **Transactional Outbox** for RabbitMQ
10. **E2E tests** (Playwright)
11. **80% coverage gate** in CI
12. **Observability stack** (OTel Collector, Loki, Grafana)

---

## 5. Validation Plan

- `dotnet build` + `dotnet test` after each task
- CI must be green on `main`
- Commit with conventional English messages and push to `origin/main`

# API Specification & OpenAPI

## 1. API Standartları (RESTful JSON)
* BingehOS yalnızca REST tabanlı JSON API sunar (GraphQL projeden çıkarılmıştır).
* Dil Desteği: İsteklerde HTTP header'ı kullanılarak i18n çevirileri sunulur.

## 2. OpenAPI 3.0 Spesifikasyon Örneği
(10 Durumlu State Machine [REJECTED dahil] ve Global `Accept-Language` konfigürasyonu içerir).

```yaml
openapi: 3.0.3
info:
  title: BingehOS API
  version: 1.0.0
servers:
  - url: https://api.bingehos.com/v1
paths:
  /work-orders:
    get:
      summary: İş emirlerini listele
      parameters:
        - $ref: '#/components/parameters/AcceptLanguage'
      responses:
        '200':
          description: Başarılı liste döndürme
    post:
      summary: Yeni iş emri oluştur
      parameters:
        - $ref: '#/components/parameters/AcceptLanguage'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                assetId:
                  type: string
                  format: uuid
                description:
                  type: string
      responses:
        '201':
          description: İş Emri DRAFT statüsünde oluşturuldu
  /work-orders/{id}/status:
    patch:
      summary: İş emri durumunu değiştir
      parameters:
        - $ref: '#/components/parameters/AcceptLanguage'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                newStatus:
                  type: string
                  enum: [DRAFT, REQUESTED, APPROVED, REJECTED, ASSIGNED, IN_PROGRESS, ON_HOLD, COMPLETED, VERIFIED, CLOSED]
      responses:
        '200':
          description: Başarılı
components:
  parameters:
    AcceptLanguage:
      in: header
      name: Accept-Language
      schema:
        type: string
        default: en-US
      description: Dönen cevabın dil formatı (i18n çevirileri için)
```

## 3. Implemented MVP Endpoints

The following routes are implemented in the current API and require an authenticated tenant context unless stated otherwise. Health endpoints are versionless: `/health`, `/health/live`, and `/health/ready`.

### Authentication

| Method | Route | Notes |
|---|---|---|
| POST | `/v1/auth/register` | Register a user; admin-only |
| POST | `/v1/auth/assign-role` | Assign a role to a user; admin-only |

### Personnel / Workers

| Method | Route | Notes |
|---|---|---|
| GET | `/v1/workers` | Paginated list; optional `activeOnly` filter |
| GET | `/v1/workers/{id}` | Get a worker |
| POST | `/v1/workers` | Create a worker; requires `employees.write` |
| PATCH | `/v1/workers/{id}` | Update worker details and active status |

### Identity administration

All user, role, and permission management endpoints require the `admin.access` permission. Deletes are soft deletes.

| Method | Route | Notes |
|---|---|---|
| GET | `/v1/users` | Paginated user list |
| GET | `/v1/users/{id}` | Get a user |
| PATCH | `/v1/users/{id}` | Update full name and active status |
| DELETE | `/v1/users/{id}` | Soft-delete a user |
| GET | `/v1/roles` | Paginated role list |
| GET | `/v1/roles/{id}` | Get a role and its permissions |
| POST | `/v1/roles` | Create a role |
| PATCH | `/v1/roles/{id}` | Update a role |
| DELETE | `/v1/roles/{id}` | Soft-delete a role |
| POST | `/v1/roles/{id}/permissions/{permissionId}` | Assign a permission to a role |
| DELETE | `/v1/roles/{id}/permissions/{permissionId}` | Soft-delete a role-permission assignment |
| GET | `/v1/permissions` | Paginated permission list |
| GET | `/v1/permissions/{id}` | Get a permission |
| POST | `/v1/permissions` | Create a permission |

### Inventory transactions

| Method | Route | Notes |
|---|---|---|
| POST | `/v1/parts/{id}/receive` | Increase stock; requires `inventory-transactions.write` |
| POST | `/v1/parts/{id}/issue` | Decrease stock; rejects negative resulting stock; requires `inventory-transactions.write` |
| POST | `/v1/parts/{id}/return` | Increase stock; requires `inventory-transactions.write` |
| GET | `/v1/inventory/transactions` | Paginated transaction list with optional `partId`; requires `inventory-transactions.read` |

The inventory balance is derived from the transaction ledger: receiving and return add quantity, while issue subtracts quantity.

### Compliance and Turkish calendar

| Method | Route | Notes |
|---|---|---|
| GET | `/v1/calibrations` | Paginated calibration records; optional `assetId`; requires `calibration-records.read` |
| GET | `/v1/calibrations/{id}` | Get a calibration record; requires `calibration-records.read` |
| POST | `/v1/calibrations` | Create a calibration record; requires `calibration-records.write` |
| PATCH | `/v1/calibrations/{id}` | Update a calibration record; requires `calibration-records.write` |
| GET | `/v1/calendar/holidays?year=2026` | Turkish fixed and religious public holidays

### Automation Studio

Automation rules are tenant-scoped and require `automation-rules.read` for reads or
`automation-rules.write` for mutations. Conditions are JSON objects with
`field`, `operator` (`equals`, `not-equals`, `greater`, or `less`), and `value`.

| Method | Route | Notes |
|---|---|---|
| GET | `/v1/automation-rules` | Paginated rule list |
| GET | `/v1/automation-rules/{id}` | Get a rule |
| POST | `/v1/automation-rules` | Create a rule |
| PATCH | `/v1/automation-rules/{id}` | Update or enable/disable a rule |
| DELETE | `/v1/automation-rules/{id}` | Soft-delete a rule |
| GET | `/v1/automation-rules/{id}/executions` | Rule execution history |

Supported triggers are `WorkOrderCreated`, `WorkOrderStatusChanged`,
`InventoryStockLow`, and `CalibrationDue`. Supported actions are
`SendNotification`, `CreateWorkOrder`, and `AdjustPriority`. Work-order and
inventory command completion publishes local MediatR notifications for the
corresponding triggers.

## 4. Route Category Status

Aşağıdaki route kategorileri için mevcut durum ve kapsam notları aşağıda güncellenmiştir. Job Plan Template mekanizması MVP'dir; Türkiye'ye özgü dört önceden doldurulmuş seed şablonu Turkey Compliance Pack içinde uygulanmıştır.

| Route Kategorisi | İlgili Bounded Context | Durum |
|---|---|---|
| `/permits` | HSE (PermitToWork, LotoProcedure) | Implemented API slice |
| `/employees` | HR & Personnel/SGK (Employee, SgkRecord, Subcontractor) | Implemented API slice |
| `/invoices` | Finance & Tax (Invoice, TaxRecord) | Implemented API slice |
| `/cost-centers` | Finance & Tax (CostCenter, MonetaryAmount) | Implemented API slice |
| `/kvkk-consents` | Compliance (KvkkConsent) | Implemented API slice |
| `/calibrations` | Compliance (CalibrationRecord) | Implemented API slice |
| `/compliance-records` | Compliance (ComplianceRecord) | Implemented API slice |

> **Not (Job Plan Template / Standart Job Library):** `/job-plan-templates` endpoint'inin **MEKANİZMASI (genel şablon altyapısı) MVP'dir** ve Doküman 01-16 kapsamında yer alır. Türkiye'ye özgü önceden doldurulmuş ekipman şablonlarının seed verisi (Asansör/Yangın/Jeneratör/HVAC) Turkey Compliance Pack içinde uygulanmıştır.
>
> Yukarıdaki implemented route'lar için tam request/response OpenAPI şemaları ayrıca genişletilebilir; route'ların kendileri artık mevcut MVP API yüzeyinin parçasıdır. Türkçe kalibrasyon CRUD, resmi tatil takvimi ve dört bakım şablonu Turkey Compliance Pack kapsamında uygulanmıştır.

## 5. Explicitly Deferred Roadmap Features

The following remain deferred and are not marked as implemented:

- Remaining Turkey Compliance Pack integrations and payroll features (e-Fatura, e-Arşiv, e-İrsaliye, e-Mutabakat, SGK, MERSİS/e-Devlet, ERP and SMS connectors)
- AI features (Copilot, predictive maintenance, report generation, spare-part recommendations)
- Digital Twin / BIM / IFC live overlays
- Turkey-specific integrations (e-Fatura, SGK, MERSİS/e-Devlet, and ERP)
- Plugin marketplace
- Mobile offline support

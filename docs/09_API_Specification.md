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

The following routes are implemented in the current API. All routes are under the `/v1` prefix and require an authenticated tenant context unless stated otherwise.

### Personnel / Workers

| Method | Route | Notes |
|---|---|---|
| GET | `/workers` | Paginated list; optional `activeOnly` filter |
| GET | `/workers/{id}` | Get a worker |
| POST | `/workers` | Create a worker; requires `employees.write` |
| PATCH | `/workers/{id}` | Update worker details and active status |

### Identity administration

All user, role, and permission management endpoints require the `admin.access` permission. Deletes are soft deletes.

| Method | Route | Notes |
|---|---|---|
| GET | `/users` | Paginated user list |
| GET | `/users/{id}` | Get a user |
| PATCH | `/users/{id}` | Update full name and active status |
| DELETE | `/users/{id}` | Soft-delete a user |
| GET | `/roles` | Paginated role list |
| GET | `/roles/{id}` | Get a role and its permissions |
| POST | `/roles` | Create a role |
| PATCH | `/roles/{id}` | Update a role |
| DELETE | `/roles/{id}` | Soft-delete a role |
| POST | `/roles/{id}/permissions/{permissionId}` | Assign a permission to a role |
| DELETE | `/roles/{id}/permissions/{permissionId}` | Soft-delete a role-permission assignment |
| GET | `/permissions` | Paginated permission list |
| GET | `/permissions/{id}` | Get a permission |
| POST | `/permissions` | Create a permission |

### Inventory transactions

| Method | Route | Notes |
|---|---|---|
| POST | `/parts/{id}/receive` | Increase stock; requires `inventory-transactions.write` |
| POST | `/parts/{id}/issue` | Decrease stock; rejects negative resulting stock; requires `inventory-transactions.write` |
| POST | `/parts/{id}/return` | Increase stock; requires `inventory-transactions.write` |
| GET | `/inventory/transactions` | Paginated transaction list with optional `partId`; requires `inventory-transactions.read` |

The inventory balance is derived from the transaction ledger: receiving and return add quantity, while issue subtracts quantity.

## 4. Route Category Status

Aşağıdaki route kategorileri için mevcut durum ve kapsam notları aşağıda güncellenmiştir. Job Plan Template mekanizması MVP'dir; Türkiye'ye özgü önceden doldurulmuş seed verileri Phase 2 kapsamındadır.

| Route Kategorisi | İlgili Bounded Context | Durum |
|---|---|---|
| `/permits` | HSE (PermitToWork, LotoProcedure) | Implemented API slice |
| `/employees` | HR & Personnel/SGK (Employee, SgkRecord, Subcontractor) | Implemented API slice |
| `/invoices` | Finance & Tax (Invoice, TaxRecord) | Implemented API slice |
| `/cost-centers` | Finance & Tax (CostCenter, MonetaryAmount) | Implemented API slice |
| `/kvkk-consents` | Compliance (KvkkConsent) | Implemented API slice |
| `/calibrations` | Compliance (CalibrationRecord) | Deferred; no current controller |
| `/compliance-records` | Compliance (ComplianceRecord) | Implemented API slice |

> **Not (Job Plan Template / Standart Job Library):** `/job-plan-templates` endpoint'inin **MEKANİZMASI (genel şablon altyapısı) MVP'dir** ve Doküman 01-16 kapsamında yer alır. Yalnızca **Türkiye'ye özgü önceden doldurulmuş (pre-seeded) ekipman şablonlarının SEED VERİSİ** (Asansör/Yangın/Jeneratör/HVAC) **Phase 2 / Turkey Compliance Pack (Doküman 17)** kapsamındadır.
>
> Yukarıdaki implemented route'lar için tam request/response OpenAPI şemaları ayrıca genişletilebilir; route'ların kendileri artık mevcut MVP API yüzeyinin parçasıdır. `/calibrations` ve Türkiye'ye özgü önceden doldurulmuş seed verileri Phase 2 kapsamındadır.

## 5. Explicitly Deferred Roadmap Features

The following remain deferred and are not marked as implemented:

- Turkey Compliance Pack and its Turkey-specific integrations
- AI features (Copilot, predictive maintenance, report generation, spare-part recommendations)
- Digital Twin / BIM / IFC live overlays
- Automation Studio, workflow designer, and rule-engine UI
- Plugin marketplace
- Mobile offline support

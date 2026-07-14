# Database Architecture & ER Diagrams

## 1. Veritabanı Mimarisi (Kesinleşen Stack)
* **Primary Relational DB:** `PostgreSQL 16` (Tenant izolasyonu RLS ile).
* **Time-Series DB:** `TimescaleDB` (PostgreSQL eklentisi).
* **Caching:** Şu anda runtime önbellek bağımlılığı provision edilmemiştir.
* **Object Storage:** `MinIO`.

## 2. Kapsamlı ER Diyagramı
*(Not: Diyagramda tekil entity isimleri (ASSET, VENDOR) kullanılırken, SQL migration scriptlerinde standart çoğul tablo isimleri (assets, vendors) kullanılmıştır.)*

```mermaid
erDiagram
    TENANT ||--o{ USER : "has"
    TENANT ||--o{ ROLE : "defines"
    TENANT ||--o{ VENDOR : "contracts"
    ORGANIZATION_UNIT ||--o{ USER : "employs"
    LOCATION ||--o{ ASSET : "houses"
    ASSET_CLASS ||--o{ ASSET : "categorizes"
    ASSET ||--o{ METER : "tracks"
    ASSET ||--o{ WORK_ORDER : "requires"
    PREVENTIVE_MAINTENANCE ||--o{ WORK_ORDER : "generates"
    JOB_PLAN ||--o{ WORK_ORDER : "templates"
    WAREHOUSE ||--o{ INVENTORY_ITEM : "stores"
    INVENTORY_ITEM ||--o{ INVENTORY_TRANSACTION : "logs"
    WORK_ORDER ||--o{ INVENTORY_TRANSACTION : "consumes"
    
    VENDOR ||--o{ CONTRACT : "signs"
    VENDOR ||--o{ WORK_ORDER : "performs"

    WORK_ORDER ||--o{ PERMIT_TO_WORK : "requires"
    PERMIT_TO_WORK ||--o{ RISK_ASSESSMENT : "assesses"
    EMPLOYEE ||--o{ SGK_RECORD : "has"
    ASSET ||--o{ CALIBRATION_RECORD : "calibrates"
    ASSET ||--o{ COMPLIANCE_RECORD : "certifies"
    TENANT ||--o{ COST_CENTER : "defines"
    INVOICE ||--o{ TAX_RECORD : "incurs"
    JOB_PLAN_TEMPLATE ||--o{ JOB_PLAN : "templates"

## 3. SQL Migration Script Örneği
```sql
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- VENDOR CONTEXT
CREATE TABLE vendors (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL
);

CREATE TABLE contracts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    vendor_id UUID NOT NULL REFERENCES vendors(id),
    title VARCHAR(255) NOT NULL,
    valid_until TIMESTAMP WITH TIME ZONE
);

ALTER TABLE contracts ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON contracts
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- HSE CONTEXT
CREATE TABLE permits_to_work (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    work_order_id UUID NOT NULL REFERENCES work_orders(id),
    permit_type VARCHAR(50) NOT NULL,
    status VARCHAR(50) DEFAULT 'DRAFT',
    approved_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE TABLE risk_assessments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    permit_to_work_id UUID REFERENCES permits_to_work(id),
    risk_level VARCHAR(50) NOT NULL,
    assessment_text TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- HR & PERSONNEL (SGK) CONTEXT
CREATE TABLE employees (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    vendor_id UUID REFERENCES vendors(id),
    full_name VARCHAR(255) NOT NULL,
    role VARCHAR(100)
);

CREATE TABLE sgk_records (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    employee_id UUID NOT NULL REFERENCES employees(id),
    sgk_registration_no VARCHAR(100),
    profession_code VARCHAR(50),
    nace_code VARCHAR(50)
);

-- FINANCE & TAX CONTEXT
CREATE TABLE invoices (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    vendor_id UUID REFERENCES vendors(id),
    invoice_type VARCHAR(50) NOT NULL,
    amount_minor BIGINT NOT NULL,
    currency CHAR(3) NOT NULL,
    issued_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE TABLE tax_records (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    invoice_id UUID NOT NULL REFERENCES invoices(id),
    tax_type VARCHAR(50) NOT NULL,
    tax_amount_minor BIGINT NOT NULL,
    currency CHAR(3) NOT NULL
);

CREATE TABLE cost_centers (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL
);

-- COMPLIANCE CONTEXT
CREATE TABLE kvkk_consents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    consent_type VARCHAR(50) NOT NULL,
    subject_ref UUID,
    consent_given BOOLEAN NOT NULL DEFAULT FALSE,
    consented_at TIMESTAMP WITH TIME ZONE
);

CREATE TABLE calibration_records (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    asset_id UUID NOT NULL REFERENCES assets(id),
    calibrated_at TIMESTAMP WITH TIME ZONE NOT NULL,
    next_due_at TIMESTAMP WITH TIME ZONE,
    result VARCHAR(50)
);

-- COMPLIANCE CONTEXT (ISO sertifikasyon kayıtları)
CREATE TABLE compliance_records (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    asset_id UUID REFERENCES assets(id),
    standard VARCHAR(50) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
    valid_until TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- MAINTENANCE CONTEXT (Standard Job Library)
CREATE TABLE job_plan_templates (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category VARCHAR(100),
    is_preseeded BOOLEAN NOT NULL DEFAULT FALSE
);

-- RLS politikaları (tüm yeni tablolar için tenant izolasyonu)
ALTER TABLE permits_to_work ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON permits_to_work
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE risk_assessments ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON risk_assessments
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE employees ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON employees
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE sgk_records ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON sgk_records
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE invoices ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON invoices
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE tax_records ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON tax_records
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE cost_centers ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON cost_centers
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE kvkk_consents ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON kvkk_consents
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE calibration_records ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON calibration_records
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE compliance_records ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON compliance_records
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

ALTER TABLE job_plan_templates ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON job_plan_templates
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

## 4. Düzeltme Notu
* **`contracts` tablosuna `tenant_id UUID NOT NULL` eklendi** (önceki sürümdeki eksiklik giderildi) ve tenant izolasyonu için RLS politikası tanımlandı. Böylece tüm tablolarda `tenant_id` + RLS zorunluluğu sağlandı.
* Yeni eklenen tablolarda tutar alanları (`invoices`, `tax_records`) `BIGINT` (minor-unit) + `currency CHAR(3)` (ISO 4217) olarak modellendi; para birimi Value Object'i (`MonetaryAmount`) ile uyumludur.

## 5. Modelleme Notları
Doküman 07'deki bazı aggregate'lerin fiziksel (SQL) karşılığı aşağıdaki gibi basitleştirilmiştir; bu, kesinleşmiş çekirdek prensipleri (RESTful-only, TimescaleDB, minor-unit `BIGINT` + `CHAR(3)`, `tenant_id` + RLS) bozmaz:

* **`LotoProcedure` ayrı bir tablo değildir:** Doküman 07 (HSE Context) aggregate'lerinden `LotoProcedure`, bağımsız bir tablo olarak modellenmez; `permits_to_work` tablosu içinde bir **tip/alan** (`permit_type` içindeki LOTO değeri ve ilgili alanlar) olarak temsil edilir.
* **`Subcontractor` ayrı bir tablo değildir:** Doküman 07 (HR & Personnel/SGK Context) aggregate'lerinden `Subcontractor`, bağımsız bir tablo olarak modellenmez; `employees` tablosundaki `vendor_id` (FK → `vendors`) ile ilgili `Vendor` kaydına bağlanarak temsil edilir.
* **`ComplianceRecord` fiziksel karşılığı:** Yukarıdaki `compliance_records` tablosudur (bkz. Bölüm 3, Compliance Context).
* **`CostCenter` fiziksel karşılığı:** Yukarıdaki `cost_centers` tablosudur (bkz. Bölüm 3, Finance & Tax Context).

-- ASSET CONTEXT
CREATE TABLE assets (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL
);

-- WORK ORDER CONTEXT
CREATE TABLE work_orders (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    asset_id UUID NOT NULL REFERENCES assets(id),
    vendor_id UUID REFERENCES vendors(id),
    status VARCHAR(50) DEFAULT 'DRAFT',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

ALTER TABLE work_orders ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_policy ON work_orders
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);
```

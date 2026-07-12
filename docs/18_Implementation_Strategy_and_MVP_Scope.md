# Implementation Strategy & MVP Scope (Kodlama Stratejisi)

## 1. Giriş
Bu doküman, FacilityOS blueprint'inin (Doküman 01-17) kodlamaya geçişi öncesi uygulama stratejisini ve ilk release (MVP) kapsamını tanımlar. Blueprint kesinleşmiştir; burada "nasıl kodlanır" sorusuna yanıt verilir.

## 2. Önerilen Solution Yapısı (Modüler Monolit)
10 bounded context, tek `FacilityOS.sln` altında modüllere ayrılır:

```
FacilityOS.sln
├── src/
│   ├── FacilityOS.Domain/           (ortak: Tenant, User, ValueObjects)
│   ├── FacilityOS.Infrastructure/   (ortak: DbContext, RLS, Redis, RabbitMQ, MinIO)
│   ├── FacilityOS.Api/              (entry point, middleware, DI, global filters)
│   └── modules/
│       ├── Identity/                (Identity & Access)
│       ├── Facility/                (Campus/Building/Space)
│       ├── Asset/                   (Asset, Meter, AssetClass)
│       ├── Maintenance/             (WorkOrder, JobPlan, PreventiveMaintenance)
│       ├── Inventory/               (Warehouse, Item, InventoryTransaction)
│       ├── Vendor/                  (Vendor, Contract)
│       ├── Hse/                     (PermitToWork, RiskAssessment)
│       ├── Personnel/               (Employee, SgkRecord)
│       ├── Finance/                 (Invoice, TaxRecord, CostCenter)
│       └── Compliance/              (KvkkConsent, ComplianceRecord, CalibrationRecord)
```

Modüller arası iletişim `MediatR` `INotification`/`INotificationHandler` (Domain Event) üzerinden — Doküman 07 ile uyumlu.

## 3. On Uygulama Önerisi

1. **Solution yapısını context'e göre böl** — Domain'leri izole tut; ileride mikroservise geçişi kolaylaştırır.
2. **Multi-Tenancy RLS'yi ilk gün kur** — `BaseEntity` + `SaveChangesInterceptor` ile `tenant_id` otomatik set; Npgsql RLS politikaları migration'da otomatik üretilsin. Sonradan 60+ tabloya eklemek 2 gün sürer.
3. **Value Object'leri sıkı kullan** — `MonetaryAmount(long MinorAmount, string Currency)` gibi `record struct`; finansal `decimal`/`float` hatasını compile-time engelle.
4. **State Machine için library kullan** — `WorkOrderStatus` + geçiş kuralları [Stateless](https://github.com/dotnet-state-machine/stateless) ile; elle `switch` yok.
5. **Mobile'i ikinci tur yap** — Önce Web API + Core domain'ler. WatermelonDB offline sync çakışma çözümü domain'i bozabilir; web API olmadan mobile testi 2x efor.
6. **Plugin altyapısını ilk sprint'te sadece yükle** — `IPlugin` interface + AssemblyLoadContext yükleme/çıkarma; Turkey Pack'i ikinci turda ekle.
7. **İlk migration'a tüm ER'yi yerleştir** — Çekirdek + Phase 2 tabloları tek seferde; sonradan eklemek versiyon/rollback'i zorlaştırır.
8. **Test piramidini solution'a göm** — `FacilityOS.UnitTests` / `IntegrationTests` (Testcontainers PG+Redis, `IClassFixture`) / `E2ETests` (Playwright). %80+ unit coverage hedefi.
9. **Observability'yi baştan kur** — Serilog/OpenTelemetry → Loki; `ActivitySource` tracing; `/health` + `/metrics` (Prometheus). Production'a çıkmadan önce zorunlu.
10. **MVP kapsamını sıkı tut** — Aşağıdaki kesim.

## 4. MVP Kapsamı (İlk Release)

### Dahil (Core / Doküman 01-16 mekanizmaları)
- Work Orders (CRUD + 9 adımlı status geçişi, E-İmza + Permit to Work zorunluluğu)
- Assets (liste, detay, web üzerinden QR/Asset Passport okutma)
- Facility (lokasyon ağacı)
- Users / Roles / Tenants (Identity + RBAC/ABAC)
- Inventory (Receiving, Issue, Return — e-Fatura hariç)
- Temel raporlama (dashboard değil, basit listeler)
- Multi-tenant RLS, logging, health/metrics

### Ertelenen (Phase 2 / Plugin / Turkey Pack)
- Turkey Compliance Pack tamamı (KVKK rıza UI, SGK, e-Fatura/e-Arşiv/e-İrsaliye, MERSİS, Türk Takvimi mesai)
- AI özellikleri (Copilot, Predictive Maintenance, AI Report Generator, Spare Part Recommendation)
- Digital Twin (BIM/IFC canlı overlay)
- Automation Studio / Workflow Designer / Rule Engine UI
- Plugin Marketplace (isteğe bağlı ilk release)
- Mobile offline (React Native + WatermelonDB)

## 5. Plugin / Phase 2 Yaklaşımı
Turkey Compliance Pack, Doküman 12'deki Marketplace plugin modeliyle (In-Process `AssemblyLoadContext` veya Out-of-Process Webhook) gelir; core'a gömülmez ("Core sektör/ülke bilmez" ilkesi). İlk sprint'te yalnızca yükleme altyapısı; paket içeriği ikinci turda.

## 6. İlk Sprint Hedefi (Çırak Testi)
`/work-orders` endpoint'leri (CRUD + status geçişi) + PostgreSQL + tenant RLS + MediatR event yayını çalışır halde olsun. Bu sağlandığında diğer modüller aynı iskelete birer modül olarak eklenir.

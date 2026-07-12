# Domain Driven Design (DDD) - Bounded Contexts

## 1. Giriş
FacilityOS, iş alanlarına (Bounded Context) bölünmüş bir mimari benimser.

## 2. Bounded Context'ler

### 2.1. Identity & Access Context (Core)
* `Tenant`, `User`, `Role`, `OrganizationUnit`.

### 2.2. Facility Context
* `Campus`, `Building`, `Space`.

### 2.3. Asset Context
* `Asset`, `AssetClass`, `Meter`.

### 2.4. Maintenance & Work Order Context
* `WorkOrder`, `JobPlan`, `PreventiveMaintenance`.
* **Standard Job Library:** Sektörlere ve iş tipine göre önceden beslenen (pre-seeded) bakım şablonları `JobPlanTemplate` aggregate root'u olarak modellenmiştir. `JobPlanTemplate`, bir `WorkOrder` oluşturulurken başlangıç şablonu olarak kopyalanır.

### 2.5. Inventory Context
* `Warehouse`, `Item`, `InventoryTransaction`.

### 2.6. Vendor & Subcontractor Context
* **Sorumluluk:** Dış hizmet sağlayıcıları, taşeron firmalar ve dış kaynaklı iş gücü yönetimi.
* **Aggregate Root'lar:**
  * `Vendor`: Dış hizmet veren şirket (Tedarikçi/Taşeron).
  * `Contract`: Şirketle yapılan SLA ve maliyet anlaşması.
* **İlişki:** Bir `WorkOrder` bir `Vendor`a atanabilir.

### 2.7. HSE / İş Sağlığı ve Güvenliği Context
* **Sorumluluk:** İş güvenliği, çalışma izinleri (permit to work), risk yönetimi ve tehlikeli iş kontrolü.
* **Aggregate Root'lar:**
  * `PermitToWork`: Çalışma izinleri (Yüksekte Çalışma, Sıcak İş/Kaynak, Kapalı Alan Girişi, Elektrik İşleri).
  * `RiskAssessment`: Görev/alan bazlı risk değerlendirmesi.
  * `LotoProcedure`: Lockout-Tagout prosedürü (enerji izolasyonu ve etiketleme).
* **İlişki:** Bir `WorkOrder` başlamadan önce, ilgili `PermitToWork` kaydının onaylı (approved) durumda olması gerekir.

### 2.8. HR & Personnel Context (SGK)
* **Sorumluluk:** Teknisyen ve personel özlük bilgileri, SGK mevzuatı, taşeron iş gücü yönetimi.
* **Aggregate Root'lar:**
  * `Employee`: Teknisyen ve personel özlük kaydı.
  * `SgkRecord`: SGK sicil bilgileri, Meslek Kodu ve NACE kodu.
  * `Subcontractor`: Alt işveren; Vendor Context'indeki bir `Vendor` ile ilişkilendirilir (alt işveren firma).
* **İlişki:** Bir `Subcontractor` bir `Vendor`a (taşeron firma) bağlanır; `Employee` ise doğrudan veya taşeron üzerinden iş gücü sağlar.

### 2.9. Finance & Tax Context
* **Sorumluluk:** Türkiye vergi mevzuatı, maliyet muhasebesi ve e-Belge süreçleri.
* **Aggregate Root'lar:**
  * `Invoice`: e-Fatura, e-Arşiv ve e-İrsaliye belgeleri.
  * `TaxRecord`: Vergi kayıtları (KDV, Stopaj, Tevkifat).
  * `CostCenter`: Masraf Merkezi (maliyet dağılımı birimi).
* **Value Object:** `MonetaryAmount` — minor-unit integer (kuruş/cent) + ISO 4217 para birimi kodu (ör. TRY, EUR).

### 2.10. Compliance Context
* **Sorumluluk:** KVKK uyumu, ISO sertifikasyon kayıtları ve denetim (audit) yönetimi.
* **Aggregate Root'lar:**
  * `KvkkConsent`: Aydınlatma Metni ve Açık Rıza kayıtları.
  * `ComplianceRecord`: ISO 55001 (Varlık), ISO 41001 (Tesis), ISO 9001 (Kalite), ISO 45001 (İSG) sertifikasyon kayıtları.
  * `CalibrationRecord`: Ölçüm cihazı ve ekipman kalibrasyon kayıtları.

> **Not (Marketplace Plugin İlkesi):** Finance/Tax (2.9), HR & SGK (2.8) ve Compliance/KVKK (2.10) gibi Türkiye'ye özgü context'ler, "Core sektör/ülke bilmez" ilkesi gereği çekirdek (core) modüle dahil edilmez. Bu context'ler **Doküman 12**'de tanımlanan **Marketplace plugin** mekanizması ile bağımsız paketler halinde dağıtılır ve tenant bazında etkinleştirilir.

## 3. Context İletişimi
Modüller arası iletişim in-memory Domain Event'ler ile (MediatR kullanılarak) sağlanır.

> **Not:** Yukarıdaki context'lerden 2.7 (HSE) ve 2.4'e eklenen `JobPlanTemplate` genel amaçlıdır ve core içinde yer alır. Türkiye-spesifik context'ler (2.8, 2.9, 2.10) için yukarıdaki Marketplace plugin notuna bakınız.

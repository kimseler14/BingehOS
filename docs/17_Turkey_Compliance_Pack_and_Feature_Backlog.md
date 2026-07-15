# Feature Backlog & Turkey Compliance Pack

> **⚠️ PHASE 2 BACKLOG:** Bu doküman **PHASE 2 BACKLOG**'dur. MVP kapsamı **Doküman 01-16**'dır. Burada listelenen tüm gelişmiş özellikler (Advanced Features) ve **Turkey Compliance Pack**, MVP tamamlandıktan sonraki Phase 2 aşamasında ele alınacaktır.

## 1. Giriş
Bu doküman, BingehOS'in global enterprise (kurumsal) özelliklerinin (Advanced Features) yanı sıra, Türkiye pazarına özel olarak geliştirilecek olan **"Turkey Compliance Pack" (Türkiye Yerel Mevzuat ve Uyum Paketi)** gereksinimlerini listeler.

## 2. Advanced Enterprise Features (Gelişmiş Platform Özellikleri)
Sistemin standart bir CMMS'in ötesine geçmesini sağlayan katma değerli özellikler:

* **Digital Commissioning:** Excel, BIM, IFC veya CAD dosyalarından binlerce ekipmanın (Asset) tek seferde toplu olarak sisteme aktarılabilmesi.
* **Facility Copilot (AI):** Doğal dil ile ("Bana geçen ayki en maliyetli 5 arızayı listele") arama, filtreleme, analiz ve profesyonel rapor oluşturabilme.
* **Automation Studio & Workflow Designer:** Teknik olmayan kullanıcıların Node-RED benzeri sürükle-bırak arayüzü ile iş akışları (Workflow) ve "Şu olursa bunu yap" otomasyon kuralları (Rule Engine) çizebilmesi.
* **Asset Passport & QR:** Cihaz üzerindeki QR kod okutulduğunda cihazın tüm geçmişi, canlı sensör verisi, dokümanları ve bakım sicilinin (Asset Health Score) tek ekranda açılması.
* **AI Destekli Bakım:** Arıza tamamlandığında otomatik profesyonel PDF raporu hazırlama (AI Report Generator) ve geçmişe dayalı yedek parça önerme (Spare Part Recommendation).
* **Maliyet ve Güvenilirlik:** Toplam sahip olma maliyeti hesaplama (Lifecycle Cost Simulator), Enerji Kaybı analizi (Energy Impact Analysis) ve gerçek zamanlı MTBF/MTTR panoları (Reliability Dashboard).
* **Dynamic Form & Dashboard Builder:** Sürükle-bırak ile tamamen özelleştirilebilir veri giriş formları, dinamik şemalar (Custom Fields) ve yönetici panoları (Custom Dashboards).
* **BIM, GIS ve Indoor Navigation:** Açık alanlar için harita (GIS), dev binalar/hastaneler için iç mekan yönlendirme (Indoor Navigation) desteği.
* **Portallar:** Misafirler için Otel/Oda arıza bildirim ekranı, Taşeronlar için Contractor Portal, Ziyaretçi takip sistemi (Visitor Management).

## 3. Turkey Compliance Pack (Türkiye Uyum Paketi)
> **Önemli:** Turkey Compliance Pack, BingehOS çekirdeğine (core) **gömülmez**; **Doküman 12 (Developer Platform, SDK & Plugins)** kapsamında bir **Marketplace plugin'i** olarak paketlenir. Core, sektör ve ülke bilgisi taşımaz; tüm Türkiye'ye özel yasal gereksinimler bu plugin aracılığıyla (Anti-Corruption Layer üzerinden) sağlanır. Böylece aynı model diğer ülke paketleri için de kullanılabilir (Bkz. Doküman 12, Bölüm 3).

Türkiye operasyonlarında yasal zorunlulukları karşılayan modüller:

### 3.1. E-Devlet, Kimlik ve Hukuki Uyum
* **KVKK Modülü:** Aydınlatma metni onayları, Açık Rıza loglaması, verilerin şifrelenmesi/anonimleştirilmesi ve yasal saklama/silme süreleri yönetimi.
* **MERSİS & E-Devlet (Faz 2):** Firma oluştururken vergi dairesi, NACE kodu ve adresin MERSİS no üzerinden otomatik çekilmesi.
* **E-İmza (E-Signature):** İş emirlerinin kapanışında yasal geçerliliği olan Mobil İmza, Elektronik İmza (USB Dongle) veya tablet üzerinden Islak İmza opsiyonları.

### 3.2. Finans ve Yerel ERP Entegrasyonları
* **Vergi ve Muhasebe:** KDV, Stopaj, Tevkifat hesaplamaları; Cari Kod ve Masraf Merkezi (Cost Center) takibi.
* **ERP Entegrasyon Hub:** Logo, Netsis, Mikro, Nebim, Canias ve SAP ile çift yönlü anlık entegrasyon.
* **E-Belge:** Satın alma ve ambar süreçlerinde e-Fatura, e-Arşiv, e-İrsaliye ve e-Mutabakat altyapısı.

### 3.3. İK, SGK ve Zaman Yönetimi
* **SGK Entegrasyonu:** Teknisyenlerin SGK sicil numaraları, Meslek Kodları, Alt İşveren (Taşeron) ilişkileri, işe giriş/çıkış takibi.
* **Türk Takvimi (uygulandı):** Resmi tatiller ve 2026-2027 dini bayram tarihleri `GET /v1/calendar/holidays?year=` ile sunulur; `TurkishWorkCalendar` iş günü hesaplamasını destekler. Mesai ve fazla mesai bordro entegrasyonları kapsam dışı ve ertelenmiştir.

### 3.4. İletişim ve Harita
* **SMS Gateway:** Netgsm, İleti Merkezi, Turkcell entegrasyonları ile OTP ve bildirim gönderimi.
* **Harita Sağlayıcıları:** Google Maps'e alternatif olarak Yandex Maps ve OpenStreetMap desteği.

## 4. İSG (İş Sağlığı ve Güvenliği) Modülü
Türkiye'deki yasal mevzuatlara tam uyumlu ayrı bir İSG alt modülü bulunmalıdır:
* **Risk Analizi ve KKD:** İş öncesi Kişisel Koruyucu Donanım (PPE) teyidi.
* **Çalışma İzinleri (Permit to Work):** Yüksekte Çalışma, Kapalı Alan, Sıcak Çalışma, Elektrik Çalışması ve LOTO (Etiketleme/Kilitleme) için dijital izin mekanizmaları.

## 5. Hazır Bakım Kütüphanesi (Standart Job Library)
> **Mekanizma / Seed Ayrımı:** `JobPlanTemplate` şablon **altyapısı (genel mekanizma ve `/job-plan-templates` endpoint'i) MVP (Doküman 03 + 07)'dir**. Türkiye'ye özgü önceden doldurulmuş ekipman şablonları artık bu paketin seed verisi olarak uygulanır.

Türkiye'de en çok kullanılan ekipmanlar için aşağıdaki dört "Hazır Şablon" sistemde tenant başına idempotent olarak seed edilir:
* **Uygulanan seed'ler:** Asansör aylık bakım, Yangın söndürme sistemleri periyodik bakımı, Jeneratör aylık bakım ve HVAC mevsimsel bakım.
* **Asansör:** Mavi Etiket / Yeşil Etiket durumu, Muayene Tarihi, A Tipi muayene takibi.
* **Yangın Söndürücüler:** Dolum tarihi, Son kontrol, Hidrostatik test süreleri, konum ve fotoğraf takibi.
* **Jeneratörler:** Haftalık test (boşta/yükte), Aylık test, Yakıt, Akü, Yağ, Filtre ve Soğutma suyu periyodik kontrolleri.
* **İklimlendirme (HVAC):** Split Klima, VRF, AHU (Klima Santrali), FCU, Chiller, Soğutma Kulesi (Cooling Tower) standart bakım ve arıza formları.
* **Enerji:** Büyük binalar için Enerji Kimlik Belgesi (EKB) sınıf takibi ve tüketim raporlaması.
* **Kalibrasyon (uygulandı):** Test cihazları ve sensörler için kalibrasyon son geçerlilik tarihi takibi ve `/v1/calibrations` CRUD API'si.

## 6. Ek Özellikler ve Tamamlayıcı Modüller (Backlog)
Aşağıdaki maddeler kullanıcının ham özellik listesinde yer almakta olup, yukarıdaki bölümlere dağılmış veya burada tamamlayıcı olarak listelenmiştir. Hepsi Phase 2 kapsamındadır.

### 6.1. Yapay Zeka, Dijital İkiz & Bilgi
* **Digital Twin (Dijital İkiz):** Varlıkların sanal ikizleri ile canlı senaryo simülasyonu (Bkz. Doküman 06).
* **Predictive Maintenance (Kestirimci Bakım):** Sensör ve geçmiş verisiyle arıza öngörüsü (Bkz. Doküman 06).
* **Warranty Intelligence (Garanti Zekası):** Cihaz garanti durumunu analiz ederek "bakım mı / değişim mi" kararını destekler (Bkz. Doküman 04 + 06).
* **Knowledge Base (Bilgi Tabanı):** Arıza çözüm makaleleri, talimat ve best-practice birikimi (Bkz. Doküman 06).

### 6.2. Analitik & Sürdürülebilirlik
* **Vendor Performance Analytics:** Tedarikçi performans skorlama ve analizi (Bkz. Doküman 04).
* **Technician Performance Analytics:** Teknisyen verimlilik ve kalite analizi (Bkz. Doküman 03 + 04).
* **Sustainability Module (Sürdürülebilirlik Modülü):** Karbon ayak izi ve yeşil bina metrikleri (Bkz. Doküman 05).

### 6.3. Platform & Altyapı
* **Event Bus (Olay Veri Yolu):** Sistem geneli olay yayınımı (Bkz. Doküman 01 + 12).
* **Webhook Platform:** Dış sistemlere olay gönderimi (Bkz. Doküman 12).
* **Offline Sync Engine (Çevrimdışı Senkronizasyon):** Mobil ve saha ekipleri için çevrimdışı çalışma ve senkronizasyon (Bkz. Doküman 10 + 01).
* **Multi-Tenant (Çok Kiracılı):** Tek kurulumda çoklu müşteri izolasyonu (Bkz. Doküman 12 + 10).
* **Plugin Marketplace:** Üçüncü parti eklenti mağazası (Bkz. Doküman 12).
* **API First:** Tüm özelliklerin API üzerinden erişilebilir olması ilkesi (Bkz. Doküman 09 + 12).
* **White Label (Marka Beyaz Etiketleme):** Müşteri markasıyla özelleştirme (Bkz. Doküman 12 + 10).

### 6.4. Kimlik, İletişim, Uyum & Yönetişim
* **SSO (Çoklu Kimlik Doğrulama):** Kurumsal kimlik sağlayıcıları ile entegrasyon (Bkz. Doküman 13 + 12).
* **Mail (E-posta Bildirimleri):** SMTP/transactional e-posta gönderimi (Bkz. Doküman 12 Marketplace connector).
* **Türkçe OCR + Genel OCR:** Türkçe karakter ve genel optik karakter tanıma (Bkz. Doküman 06 + 12).
* **Compliance Center (Uyum Merkezi):** Tüm uyum kayıtlarının merkezi yönetimi (Bkz. Doküman 13 + 07 Compliance).
* **Audit Management (Denetim Yönetimi):** Denetim planı ve bulgu takibi (Bkz. Doküman 13).
* **Contract Management (Sözleşme Yönetimi):** Bakım ve servis sözleşmelerinin yaşam döngüsü (Bkz. Doküman 04).

## 7. İzlenebilirlik / Kapsam Haritası (Traceability Matrix)
Aşağıdaki tablo, kullanıcı ham özellik listesindeki her maddenin hangi dokümanda ve (varsa) hangi bounded context'te spesifiye edileceğini gösterir. Böylece hiçbir madde "sahipsiz" kalmaz.

| Özellik / Grup | Hedef Doküman | Bounded Context (Doküman 07) |
|---|---|---|
| Facility Copilot (AI) | 06 | — (AI & Analytics) |
| Automation Studio | 01 + 12 | — |
| Digital Twin | 02 + 06 | — (Asset + AI) |
| Asset Passport & QR | 02 + 06 | — (Asset) |
| Predictive Maintenance | 06 | — (AI) |
| AI Report Generator | 06 | — |
| Spare Part Recommendation | 06 | — |
| Warranty Intelligence | 04 + 06 | — |
| Lifecycle Cost Simulator | 04 + 06 | — |
| Energy Impact Analysis | 05 | — (Energy/IoT) |
| Reliability Dashboard (MTBF/MTTR) | 03 + 06 | — |
| Asset Health Score | 02 + 06 | — (Asset) |
| Knowledge Base | 06 | — |
| Standard Job Library (şablon mekanizması) | 03 + 07 | Maintenance/WorkOrder (JobPlanTemplate) – **MVP** |
| Dynamic Form Builder | 11 + 01 | — |
| Workflow Designer | 01 + 12 | — |
| Rule Engine | 01 + 12 | — |
| Event Bus | 01 + 12 | — |
| Webhook Platform | 12 | — |
| Integration Hub (ERP Hub) | 12 | — |
| Offline Sync Engine | 10 + 01 | — |
| Multi-Tenant | 12 + 10 | — |
| Plugin Marketplace | 12 | — |
| Custom Fields | 11 + 01 | — |
| Custom Dashboard Builder | 11 | — |
| BIM / IFC | 02 | — (Facility/Asset) |
| GIS | 02 | — (Facility/Asset) |
| Indoor Navigation | 02 | — (Facility/Asset) |
| Contractor Portal | 01 | — (Portal) |
| Visitor Management | 01 | — (Portal) |
| Permit to Work | 03 + 07 | HSE (PermitToWork) |
| Calibration Management | 03 / 05 + 07 | Compliance (CalibrationRecord) – **Implemented CRUD** |
| Compliance Center | 13 + 07 | Compliance (ComplianceRecord) |
| Audit Management | 13 | — (Security & Compliance) |
| Contract Management | 04 | — (Inventory/Financial) |
| Vendor Performance Analytics | 04 | — |
| Technician Performance Analytics | 03 + 04 | — |
| Cost Center Management | 04 + 07 | Finance & Tax (CostCenter) |
| Sustainability Module | 05 | — (Energy/IoT) |
| BingehOS SDK | 12 | — |
| White Label | 12 + 10 | — |
| API First | 09 + 12 | — |
| Otel/Oda Misafir Arıza Bildirimi | 01 | — (Portal) |
| Hazır Bakım Kütüphanesi (AHU/FCU/VRF/Chiller/Jeneratör/UPS/Yangın Pompası/Hidrofor/Asansör) | 03 + 05 | — (Maintenance/Asset) |
| KVKK | 13 + 07 | Compliance (KvkkConsent) |
| E-İmza | 13 | — (Security) |
| e-Devlet | 13 + 12 | — (Connector) |
| MERSİS | 07 + 13 | Finance & Tax / Compliance |
| Vergi (KDV/Stopaj/Tevkifat) | 04 + 07 | Finance & Tax (TaxRecord) |
| Logo / Netsis / Mikro / Nebim / Canias / SAP | 12 | — (Out-of-Process Connector / ACL) |
| e-Fatura / e-Arşiv / e-İrsaliye / e-Mutabakat | 07 + 12 | Finance & Tax (Invoice) |
| SGK (sicil / meslek kodu / alt işveren) | 07 | HR & Personnel/SGK (Employee, SgkRecord, Subcontractor) |
| İSG (Risk / KKD / LOTO / Çalışma İzinleri) | 03 + 07 | HSE (RiskAssessment, LotoProcedure) |
| Yangın Söndürücü Takibi | 03 + 05 | — (Asset) |
| Asansör (Mavi/Yeşil Etiket) | 02 + 03 | — (Asset/Maintenance) |
| Jeneratör Şablonu | 03 | — (Maintenance) |
| Klima (Split/VRF/AHU/FCU/Chiller/Cooling Tower) | 03 | — (Maintenance) |
| Enerji Kimlik Belgesi (EKB) | 05 | — (Energy/IoT) |
| SMS (Netgsm/İleti Merkezi/Turkcell) | 12 | — (Out-of-Process Connector) |
| OTP | 13 + 12 | — (Security/Connector) |
| Mail | 12 | — (Out-of-Process Connector) |
| SSO | 13 + 12 | — (Security/Connector) |
| Harita (Google/Yandex/OpenStreetMap) | 12 | — (Out-of-Process Connector) |
| Türkçe OCR + Genel OCR | 06 + 12 | — |
| Türk Takvimi (resmi tatil/dini bayram/mesai/fazla mesai/hafta tatili) | 15 | Compliance (`TurkishWorkCalendar`, 2026-2027 holidays) – **partial; payroll/overtime deferred** |

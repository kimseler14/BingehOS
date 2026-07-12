# Developer Platform, SDK & Plugins

## 1. Giriş
FacilityOS'in üçüncü parti entegratörler ve müşterilerin kendi IT ekipleri tarafından genişletilebilmesi için kurulan platform katmanıdır.

## 2. Plugin (Eklenti) Mimarisi
Modüler Monolit (.NET 8) içerisinde dışarıdan özellik eklemenin iki yolu vardır:

* **In-Process Plugins (Sistem İçi Eklentiler):**
  * `.NET Assembly (DLL)` yükleme altyapısı (Örn: `System.Runtime.Loader.AssemblyLoadContext`) kullanılarak geliştirilen paketler sisteme entegre edilir.
  * Eklentiler, sistemdeki temel aksiyonlara (Örn: `WorkOrderCreatedEvent`) MediatR pipeline'ı üzerinden abone (Subscriber) olabilir.
* **Out-of-Process Plugins (Dış Sistem Eklentileri):**
  * Eklenti kendi sunucusunda çalışır. FacilityOS ile iletişimini Webhook'lar ve Developer REST API'si üzerinden sağlar. (Önerilen güvenli entegrasyon yöntemi).

## 3. Marketplace (Uygulama Mağazası)
* Kurumsal müşteriler `Marketplace` sekmesinden onaylanmış entegrasyonları tek tıkla kurabilirler. Örnek onaylı paketler:
  * **"SAP ERP Connector"** — Kurumsal ERP entegrasyonu.
  * **"Siemens BMS IoT Bridge"** — Bina otomasyon sistemi (BMS/IoT) köprüsü.
  * **"Turkey Compliance Pack"** — KVKK, SGK, e-Fatura, MERSİS ve Türk Takvimi gibi Türkiye'ye özel mevzuat ve yerel gereksinimleri içeren resmi bölgesel (regional) plugin. (Bkz. Doküman 17)
* Marketplace paketleri temelde uygulamanın hangi Webhook'ları dinleyeceğini ve hangi API yetkilerine (Scopes) sahip olacağını belirleyen manifest (`manifest.json`) dosyalarıdır.

### 3.1. ERP & Yerel Sistem Connector'ları (Out-of-Process Model)
Yerel muhasebe/ERP sistemleri (Logo, Netsis, Mikro, Nebim, Canias, SAP) **Out-of-Process / Marketplace connector** modeliyle sunulur. Bu connector'lar FacilityOS çekirdeğine (core) gömülmez; kendi süreçlerinde (out-of-process) çalışır ve **Anti-Corruption Layer (ACL)** üzerinden çevrilmiş bir sözleşmeyle iletişim kurar. Böylece core, sektör ve ülke bilgisi taşımaz (Core sektör/ülke bilmez ilkesi).

### 3.2. Bölgesel Mevzuat Paketleri (Diğer Ülkeler)
"Turkey Compliance Pack" tekil bir istisna değildir; Almanya, Fransa, Körfezdeki ülkeler vb. diğer ülkelerin yasal mevzuat paketleri de **aynı Marketplace plugin modeliyle** eklenebilir. Tüm bölgesel uyum paketleri tutarlı bir connector/plugin sözleşmesi üzerinden çalışır.

## 4. CLI Aracı (Command Line Interface)
Geliştiriciler için `facility-cli` aracı sağlanır:
* `facility-cli auth login` -> Geliştirici kimlik doğrulaması.
* `facility-cli plugin create my-plugin` -> Yeni bir eklenti iskeleti oluşturur.
* `facility-cli webhook listen` -> Local ortamda (ngrok benzeri) webhook olaylarını test edebilmeyi sağlar.

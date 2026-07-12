# Localization & Internationalization (i18n)

## 1. Çoklu Dil Mimarisi
* **UI Çevirileri:** Next.js ve React Native `i18next` kütüphanesi ile çalışır.
* **Dinamik Veri Çevirileri:** Veritabanındaki tanımlamalar (Örn: Kategori İsimleri) `JSONB` formatında tutulur (`{"en": "Air Handling Unit", "tr": "Klima Santrali"}`). API, isteği yapanın `Accept-Language` header'ına göre uygun dili döndürür.

## 2. Timezone
* Tüm Timestamp verileri veritabanında `UTC` olarak tutulur. Render işlemi UI tarafında, kullanıcının profilindeki Timezone ayarına göre yapılır.

## 3. Para Birimi ve Hassasiyet (Currency Validation)
* **Minor-Unit Integer:** JavaScript/C# double veri tiplerindeki yuvarlama/kayan nokta hatalarını (Floating-point error) engellemek için finansal tutarlar veritabanında **Integer** olarak saklanır (Kuruş/Cent bazında).
  * Örn: `15.50 USD` -> DB'de `1550` olarak tutulur.
* **ISO 4217 Para Birimi Kodu (CHAR(3)):** Minor-unit integer kuralına ek olarak her tutar, **ISO 4217** standardına uygun 3 haneli para birimi kodu ile (**currency CHAR(3)**) etiketlenir. Böylece çok para birimli ortamlarda tutar ve para birimi birlikte tutarlı saklanır (Doküman 04 §3.3 ile tutarlı).
  * Örn: `15.50 USD` -> `amount = 1550`, `currency = 'USD'`.
* **Kur Dönüşümü Yaklaşımı:** Tutarlar yerel para biriminde (minor-unit) saklanır. Farklı para birimleri arası dönüşüm, yalnızca raporlama/toplama anında **dönem bazlı kur** (exchange rate) üzerinden hesaplanır; dönüşüm sonucu da minor-unit integer olarak yuvarlanır. Kur geçmişi (rate history) zaman serisi olarak tutulur ve dönüşümde işlemin tarihindeki geçerli kur kullanılır.

## 4. Türk Takvimi ve Çalışma Süresi Hesaplamaları (Turkey Calendar Context)
> **Not:** Aşağıdaki Türkiye'ye özgü takvim ve mevzuat kuralları, Core'dan ayrıştırılarak **Marketplace plugin'i** (Doküman 12) ve **Turkey Compliance Pack** (Doküman 17) kapsamında gelecektir; Core ülke/sektör bilmez.

* **Gömülü (Seeded) Takvim:** Türkiye'ye ait **resmi tatiller** ve **dini bayramlar** (Ramazan Bayramı, Kurban Bayramı vb.), tarih aralıkları ile birlikte sistem içine **seed data** olarak yüklenir.
* **Mesai / Fazla Mesai / Hafta Tatili:** Çalışanın normal mesai süresi, fazla mesai (overtime) ve hafta tatili hesaplamaları, bu gömülü Türk takvimine göre **otomatik** olarak yapılır; resmi tatil ve bayram günlerine denk gelen işlemler ilgili katsayı/ücret kuralına otomatik atanır.
* **Yapılandırılabilirlik:** Tatil takvimi, kurumun lokasyonuna göre (örn. yarım gün tatiller) özelleştirilebilir; yeni takvim kayıtları plugin üzerinden eklenebilir.

## 5. Diğer Ülke Paketleri (Plugin Mimarisi)
* Türkiye takvimi ve mevzuatı gibi diğer ülkelere ait takvim, tatil ve yasal mevzuat paketleri de **plugin** (Doküman 12) olarak sisteme eklenebilir. Core, ülke-spesifik kuralları bilmez; ilgili plugin aktifken kendi takvim/mevzuat kurallarını enjekte eder.

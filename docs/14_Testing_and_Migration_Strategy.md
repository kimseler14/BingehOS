# Testing & Migration Strategy

## 1. Testing Strategy (Test Planı)
Modüler Monolit yapı için test stratejisi üç katmandan oluşur (Test Piramidi):

* **Unit Tests (Birim Testler):** xUnit/NUnit kullanılarak, özellikle Domain içerisindeki State Machine geçişleri (Örn: `WorkOrder.Start()` metodunun kuralları) mock'lanarak test edilir. Code coverage hedefi: %80+.
* **Integration Tests (Entegrasyon Testleri):** Testcontainers kullanılarak gerçek PostgreSQL ayağa kaldırılır. Veritabanı kayıtları ve Modüller arası event fırlatmaları test edilir.
* **E2E Tests:** Playwright ile kritik senaryolar (Kullanıcı girişi -> İş Emri Oluşturma -> Kapatma) tarayıcı üzerinde otomatize edilir.
* **Performance Testing:** k6 kütüphanesi kullanılarak API uçlarına ve WebSocket/IoT telemetri uçlarına yük testi yapılır.

## 2. Migration Strategy (Eski Sistemlerden Geçiş)
Geleneksel CMMS'lerden BingehOS'e geçiş için standartlaştırılmış bir ETL (Extract, Transform, Load) aracı hazırlanacaktır.

* **Excel/CSV Import Aracı:** Kullanıcıların Asset, Location ve Parts verilerini şablon Excel dosyalarıyla sisteme yüklemesini sağlayan bir arayüz. Yükleme sırasında veri doğrulama (Validation) yapılır.
* **Legacy API Konektörleri:** SAP PM, IBM Maximo gibi sistemlerden tarihi Work Order verilerini çekip BingehOS formatına dönüştüren arka plan (Background Worker) servisleri. Veri aktarımı sırasında tenant isolation kuralları %100 işletilir.

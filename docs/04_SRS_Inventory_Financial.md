# Software Requirements Specification (SRS) - Inventory & Financial Modules

## 1. Giriş
Envanter (Inventory) ve Finansal Yönetim (Financial) modülleri, tesis içindeki tüm yedek parça, sarf malzeme stoklarının yönetimi ve tüm bu bakım/tesis işlemlerinin maliyet analizlerinin yapılmasını sağlar.

---

## 2. Inventory (Envanter ve Stok Yönetimi) Modülü
Bu modül, depolardaki malzemelerin giriş, çıkış ve transfer süreçlerini izler.

### 2.1. Depo Hiyerarşisi
* **Hiyerarşi:** Warehouse (Ana Depo) > Location (Bölüm) > Shelf (Raf) > Bin (Kutu).
* **Çoklu Depo:** Bir kampüste birden fazla ambar olabilmeli ve ambarlar arası "Stock Transfer" işlemi yapılabilmelidir.

### 2.2. Stok Kalemleri ve İşlemler
* **Stok Tipleri:** Yedek Parça (Spare Part), Sarf Malzeme (Consumable), Hırdavat/Alet (Tool).
* **İşlemler:** 
  * **Receiving (Mal Kabul):** Satın alınan ürünlerin depoya girilmesi.
  * **Issue (Çıkış):** İş emrine veya bir departmana malzeme verilmesi.
  * **Return (İade):** Kullanılmayan malzemenin depoya iade alınması.
  * **Reservation (Rezerve Etme):** İleri tarihli planlı bir bakım için stoktan parça ayırtılması.
* **Barkod/QR Destekli Stok Sayımı (Cycle Count):** Mobil cihazlar ile periyodik ambar sayımlarının kolayca yapılabilmesi.

### 2.3. Satın Alma Döngüsü (Procurement)
* Kritik stok seviyesinin altına düşen malzemeler için otomatik **Purchase Request (Satın Alma Talebi)** oluşturulabilmelidir.
* Taleplerin onaya gitmesi, onaylanınca **Purchase Order (Satın Alma Siparişi)** formuna dönüşmesi sağlanmalıdır.
* Tedarikçi (Vendor) katalogları ve sözleşmeleri (Contracts) sisteme yüklenebilmelidir.

---

## 3. Financial (Maliyet ve Finans) Modülü
BingehOS'te yapılan her işlemin bir maliyeti (TCO - Toplam Sahip Olma Maliyeti) hesaplanmalıdır.

### 3.1. Maliyet Kalemleri
* **Labor Cost (İşçilik Maliyeti):** Teknisyenin saatlik ücreti x İş Emri Süresi.
* **Material Cost (Malzeme Maliyeti):** İş emrinde kullanılan yedek parçaların güncel FIFO/LIFO depo maliyeti.
* **Vendor Cost (Dış Hizmet Maliyeti):** Alt yüklenicilere ödenen fatura bedelleri.
* **Downtime Cost (Duruş Maliyeti):** Bir üretim bandının veya hastane odasının servis dışı kaldığı her saat başına kaybettiği para tutarı.
* **Energy Cost:** Varlığın tükettiği enerji faturası.

### 3.2. Bütçe ve Maliyet Merkezi Yönetimi
* Şirketin her bir tesisi, katı veya departmanı bir "Cost Center" (Maliyet Merkezi) olarak tanımlanabilmelidir.
* Yıllık bütçeler (CAPEX - Sermaye Gideri, OPEX - İşletme Gideri) tanımlanıp, iş emirleri tamamlandıkça gerçekleşen bütçeler canlı olarak kıyaslanabilmelidir.
* Bir varlığı tamir etmenin maliyeti, onun güncel "Asset Value" (Varlık Değeri) limitini aştığında sistem kullanıcıyı uyarmalıdır. (Örn: "Bu cihazı tamir etmek yerine yenisini almak (ROI) daha kârlı.")

### 3.3. Para Birimi Hassasiyeti
* Tüm finansal tutarlar (malzeme maliyeti, işçilik, fatura bedeli, bütçe vb.), kayan nokta (floating-point) hatasını tamamen elimine etmek için veritabanında **minor-unit integer** (**BIGINT**) olarak saklanır; para birimi ise **ISO 4217** kodu ile (**currency CHAR(3)**) tutulur (Doküman 15 ile tutarlı).
  * Örn: `15.50 USD` -> `amount = 1550`, `currency = 'USD'`.
* Çok para birimli ortamlarda her işlem kendi para birimi kodu ile saklanır; kur dönüşümleri yalnızca raporlama anında ilgili dönem kuru üzerinden hesaplanır (bkz. Doküman 15).

---

## 4. Türkiye Vergi Mevzuatı (Turkey Tax Context)
> **Not:** Aşağıdaki Türkiye'ye özgü tüm vergi, belge ve mevzuat gereksinimleri, Core'dan ayrıştırılarak **Turkey Compliance Pack plugin'i** (Doküman 17 & 12) kapsamında gelecektir. Core, sektör ve ülke bilgisi taşımaz; bu kurallar plugin aracılığıyla enjekte edilir.

### 4.1. Vergi Hesaplama Gereksinimleri
* **KDV (Katma Değer Vergisi):** Satın alma, ambar çıkışı ve faturalama süreçlerinde uygulanacak KDV oranları (standart %20, indirimli %10/%1/%) tanımlanabilmeli, KDV tutarları her işlem için ayrı satır olarak hesaplanabilmelidir.
* **Stopaj (Withholding Tax):** Dış hizmet/fatura bedelleri üzerinden uygulanabilecek stopaj oranları yapılandırılabilmeli ve ilgili muhasebe kaydına yansıtılabilmelidir.
* **Tevkifat:** KDV tevkifatı (kısmi kesinti) senaryoları desteklenmeli; tevkifata tabi işlemlerde alıcı/satıcı payları doğru hesaplanmalıdır.
* **Cari Kod (Vendor/Customer Account Code):** Her tedarikçi ve müşteri için Türkiye muhasebe standartlarına uygun **Cari Kod** takibi zorunludur; tüm finansal hareketler ilgili cari koda bağlanabilmelidir.
* **Masraf Merkezi (Cost Center) Takibi:** Maliyet merkezi tanımı (bkz. §3.2) Türkiye muhasebesinde "Masraf Merkezi" olarak da etiketlenebilmeli ve her fiş/muhasebe hareketi bir masraf merkezine atanabilmelidir.

## 5. E-Belge Altyapısı (Finance & Tax Context)
Satın alma ve ambar süreçleri, Türkiye e-Belge sistemi ile entegre çalışabilmelidir. Bu altyapı **Turkey Compliance Pack plugin'i** (Doküman 17 & 12, Finance & Tax context) kapsamında sağlanır:
* **e-Fatura:** Tedarikçi faturalarının Gelir İdaresi Başkanlığı (GİB) portalı üzerinden alım/çıkış işlemleri; sistemdeki Purchase Order ve Receiving kayıtları ile eşleşebilmelidir.
* **e-Arşiv:** Son kullanıcıya/B2C kesilen faturaların e-Arşiv olarak saklanması ve GİB ile senkronizasyonu.
* **e-İrsaliye:** Mal kabul (Receiving) ve Stock Transfer işlemlerinde sevk irsaliyesinin e-İrsaliye olarak oluşturulması ve araç/transfer bilgisi ile ilişkilendirilmesi.
* **e-Mutabakat:** Cari hesaplar için periyodik e-Mutabakat (mutabakat mektubu) üretimi ve tedarikçi/müşteri tarafıyla teyidi.

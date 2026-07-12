# Software Requirements Specification (SRS) - Facility & Asset Modules

## 1. Giriş
Facility (Mekan/Tesis) ve Asset (Demirbaş/Varlık) modülleri, platformun fiziksel dünyayı dijitalleştirdiği alanlardır. Bu modüller birbirleriyle sıkı bir ilişki içerisindedir; varlıklar (assets) genellikle tesislerdeki belirli konumlarda (facilities) bulunur.

---

## 2. Facility (Tesis ve Uzamsal Yönetim) Modülü
Bu modül, organizasyonun sahip olduğu veya yönettiği fiziksel alanların hiyerarşik olarak tanımlanmasını sağlar.

### 2.1. Hiyerarşik Alan Yönetimi (Spatial Hierarchy)
Sistem esnek ve iç içe geçebilen bir lokasyon ağacını desteklemelidir.
* **Standart Seviyeler:** `Campus` > `Building` > `Floor` > `Zone` > `Room/Area`.
* **Ek Alanlar:** Açık alanlar, otopark, çatı (roof), koridor (corridor), bodrum (basement) vb. tipler sistemde tanımlanabilmelidir.
* **Kalıtım (Inheritance):** Bir binaya atanan bir özellik veya kural, aksi belirtilmedikçe altındaki odalara veya katlara miras olarak aktarılmalıdır.

### 2.2. Harita ve Plan Entegrasyonu
* **Kat Planları (Floor Plans):** Kullanıcılar 2D/PDF/Görsel kat planlarını yükleyebilmeli ve üzerlerine interaktif pinler (Varlıklar, sensörler) yerleştirebilmelidir.
* **GIS Desteği:** Kampüs ve bina lokasyonları harita üzerinde (Enlem/Boylam ve GeoJSON poligonları ile) tutulabilmelidir.
* **BIM & 3D (Building Information Modeling):** İleri aşamalarda IFC uzantılı 3D BIM modellerinin sisteme yüklenip görüntülenebileceği altyapıya uygun veri şeması hazırlanmalıdır (Digital Twin hazırlığı).
* **İç Mekan Yönlendirme (Indoor Navigation):** Büyük binalar (hastane, kampüs, AVM, fabrika) için kat planı/BIM üzerinde varlıkların ve hedef lokasyonların iç mekan yönlendirme (rota çizimi, katlar arası geçiş) ile bulunabilmesi desteklenmelidir. Teknisyen mobil uygulamadan bir arızalı varlığı seçtiğinde sisteme iç mekan rotası (turn-by-turn) ile yönlendirilmelidir.

---

## 3. Asset (Varlık ve Demirbaş) Modülü
Bu modül, tesis içindeki tüm cihazların, ekipmanların ve envanterlerin yaşam döngüsünü yönetir.

### 3.1. Varlık Kategorizasyonu ve Şablonlar
* **Asset Class & Type:** Varlıklar bir sınıf (Örn: HVAC) ve tipe (Örn: Klima Santrali) göre kategorize edilmelidir.
* **Asset Templates (Şablonlar):** Aynı tipteki varlıkların (Örn: 100 adet aynı marka/model pompa) sisteme kolayca girilebilmesi için önceden tanımlanmış şablonlar kullanılabilmelidir.

### 3.2. Varlık İlişkileri (Asset Relationships)
Varlıklar sadece lokasyona değil, birbirlerine de bağlanabilmelidir.
* **Parent-Child (Bileşen İlişkisi):** Bir jeneratörün (Parent) içindeki motor (Child) veya filtreler bileşen olarak tanımlanabilmelidir.
* **Bağımlılık (Dependency):** Elektrik panosu A bozulduğunda Pompa B'nin çalışmayacağını gösteren bağlantı ağaçları (Topology) oluşturulabilmelidir.

### 3.3. Varlık Yaşam Döngüsü (Lifecycle Management)
Her bir varlık için aşağıdaki veriler tarihçeli olarak tutulmalıdır:
* **Durum:** Aktif, Pasif, Bakımda, Hurda, Depoda.
* **Finansal Veriler:** Satın alma tarihi, garanti bitiş tarihi (Warranty), amortisman (Depreciation) planı.
* **Sağlık Skoru (Health Score):** Arıza geçmişi, yaşı ve bakım sıklığına göre sistem tarafından otomatik hesaplanan bir 1-100 sağlık endeksi olmalıdır.

### 3.4. Etiketleme ve Tanıma (Identification)
* Her varlığa otomatik olarak benzersiz bir kod (Asset Tag) atanmalıdır.
* Sistem QR Kod, Barkod, RFID ve NFC taramalarıyla uyumlu olacak şekilde URI yapısı sunmalıdır (Mobil uygulamadan okutulduğunda varlık detayının açılması).

### 3.5. Arıza ve Bakım Geçmişi (Failure History)
* Bir varlığın sicilinde ona atanmış tüm geçmiş İş Emirleri (Work Orders), yapılan masraflar (TCO - Total Cost of Ownership) ve yedek parça kullanımları tek bir ekrandan izlenebilmelidir.

### 3.6. Digital Commissioning (Toplu İçe Aktarma / Devreye Alma)
Yeni bir tesis devreye alınırken veya mevcut tesis sisteme ilk kez tanıtılırken binlerce ekipmanın (Asset) manuel tek tek girilmesi yerine **toplu içe aktarılabilmesi** zorunludur. Sistem şu kaynaklardan toplu içe aktarmayı (bulk import) desteklemelidir:
* **Excel/CSV:** Standartlaştırılmış şablon (asset import template) ile toplu varlık girişi; hatalı satırlar için doğrulama (validation) raporu ve kısmi geri alma.
* **BIM / IFC:** Building Information Modeling modellerinden (IFC uzantılı dosyalar) ekipman, tip, konum ve nitelik (attributes) bilgilerinin otomatik çıkarılıp Asset olarak oluşturulması.
* **CAD:** 2D kat planı/CAD dosyalarından blok ve katman bilgilerine göre varlık ve konum eşlemesi.
* İçe aktarma sırasında varlıklar otomatik olarak hiyerarşik lokasyona (Spatial Hierarchy) bağlanmalı, Asset Tag, QR Kod ve başlangıç sağlık skoru otomatik atanmalıdır.

### 3.7. Asset Passport & QR (Varlık Pasaportu)
Her varlık, saha teknisyeni veya denetleyici tarafından **QR kodu okutulduğunda** tüm kritik bilgilerin tek ekranda (Asset Passport) açıldığı bir "dijital varlık sicili" sunmalıdır. QR okutulduğunda açılan ekran şunları içermelidir:
* **Geçmiş (History):** Atanmış tüm İş Emirleri, arıza ve bakım sicili, TCO.
* **Canlı Sensör Verisi:** TimescaleDB üzerinde tutulan gerçek zamanlı/geçmiş sensör ölçümleri ve trend grafikleri.
* **Garanti (Warranty):** Satın alma tarihi, garanti bitiş tarihi, tedarikçi bilgisi.
* **Dokümanlar:** Kullanım kılavuzu, bakım el kitabı, teknik resim, sertifikalar (MinIO'da tutulan dosyalar).
* **Bakım Sicili / Sağlık Skoru (Asset Health Score):** Sistemin otomatik hesapladığı 1-100 sağlık endeksi ve son durum.
Bu ekran mobil uygulamadan (React Native) ve web arayüzünden erişilebilir olmalıdır.

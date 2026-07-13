# Software Requirements Specification (SRS) - AI, Analytics & Digital Twin Modules

## 1. Giriş
Bu doküman, BingehOS'in veriyi bilgiye, bilgiyi ise öngörüye dönüştürdüğü ileri düzey teknoloji modüllerini kapsar: Yapay Zeka (AI), Gelişmiş Analitik ve Dijital İkiz (Digital Twin).

---

## 2. AI Platform (Yapay Zeka) Modülü
AI katmanı, tesis operasyonlarını hızlandırmak ve insan hatasını minimize etmek için kullanılır.

### 2.1. AI Assistant & Knowledge Base
* **RAG (Retrieval-Augmented Generation):** Sistemde yüklü olan tüm kullanım kılavuzları (Manuals), bakım prosedürleri ve geçmiş arıza notları vektörel veritabanında saklanmalıdır.
* Teknisyen sahada uygulamaya "Bu asansörün kapı motoru arızası için geçmişte ne yapmıştık?" diye sorduğunda (LLM destekli chat arayüzü), sistem kendi dokümanlarını tarayarak yanıt ve çözüm önerisi getirebilmelidir.

### 2.2. Tahmine Dayalı İşlemler (Predictive AI)
* **Failure Prediction (Arıza Tahmini):** Cihazın yaş, titreşim (IoT) ve çalışma saatine bakarak ne zaman bozulabileceğini tahmin eden makine öğrenimi modeli.
* **Smart Scheduling:** Teknisyenlerin yetenek seti, konumu ve işin aciliyetine göre günlük bakım rotalarının yapay zeka tarafından otomatik oluşturulması.

### 2.3. Görsel ve Ses İşleme (Computer Vision & Audio)
* **OCR:** Tedarikçiden gelen irsaliye ve faturaların fotoğrafı çekildiğinde sistemin içindeki kalemleri otomatik okuması (Inventory/Financial).
* **Speech To Text:** Teknisyenin elleri doluyken mobil uygulamaya konuşarak iş emri kapanış notunu (sesle) yazdırabilmesi.

---

## 3. Digital Twin (Dijital İkiz) Modülü
Fiziksel tesisin dijital, canlı kopyasının görselleştirilmesi.

### 3.1. Çok Katmanlı 3D Görselleştirme
* IFC ve 3D BIM modelleri bir web görüntüleyicide (Örn: Forge, xeokit) izlenebilmelidir.
* **Overlay (Katmanlar):** 
  * Varlık Katmanı: Hangi klima nerede duruyor?
  * Alarm Katmanı: Hangi odada arızalı (kırmızı yanan) bir sistem var?
  * Enerji Katmanı: Binanın hangi bölümü çok fazla ısınmış (Heatmap).

### 3.2. Canlı Navigasyon
* Dev kampüslerde veya hastanelerde bir iş emri atandığında teknisyene "Bulunduğun noktadan arızalı cihaza giden en kısa kapalı alan rotası" (Indoor Navigation) çizilebilmelidir.

---

## 4. Analytics & Dashboard Modülü
### 4.1. İş Zekası (BI) ve Dashboard
* Sürükle-bırak widget'lar ile kişiselleştirilebilir Dashboard (Yönetici, Bakım Müdürü, Teknisyen için ayrı görünümler).
* **Kritik KPI'lar:** MTTR, MTBF, Availability (Kullanılabilirlik Oranı), OEE (Toplam Ekipman Etkinliği), SLA Başarı Oranı.
* Veriler farklı periyotlarla (Günlük, Haftalık) otomatik olarak PDF raporları halinde ilgili yöneticilere e-posta ile gönderilebilmelidir.

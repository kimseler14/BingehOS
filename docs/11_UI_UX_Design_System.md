# UI/UX Design System & Developer Platform

## 1. UI/UX Design System (Arayüz ve Deneyim Sistemi)
BingehOS gibi veri yoğun ve ekranı çok olan uygulamalarda tutarlılığı sağlamak için kendi "Design System"ini (Bileşen kütüphanesini) kurmak şarttır.

### 1.1. Görsel Dil ve Temalar
* **Kurumsal ve Temiz Arayüz:** Aşırı renk karmaşasından uzak, okunabilirliği yüksek (Veri tabloları, yoğun formlar) bir arayüz.
* **Tema Desteği:** Dark Mode ve Light Mode varsayılan olarak desteklenmelidir. Enterprise müşteriler için CSS variable'ları üzerinden (Örn: Şirket logosu, ana marka rengi) Whitelabel (markalama) özelliği sunulmalıdır.
* **Responsive (Duyarlı) Tasarım:** Web arayüzü masaüstü odaklı olsa da, tablet teknisyenleri için iPad / Tablet boyutlarında kusursuz çalışmalıdır.

### 1.2. Bileşen Kütüphanesi (Component Library)
* Sisteme özel Butonlar, Tablolar (Datagrid), Form Elemanları (Dropdown, Datepicker), Modallar, Card'lar standartlaştırılmalı ve Storybook gibi bir araçla dökümante edilmelidir.
* **DataGrid (Tablo) Yetenekleri:** Tüm listeleme ekranlarında kolon gizleme/gösterme, çoklu sıralama (multi-sort), gelişmiş filtreleme ve Excel'e aktarım standart bileşen özelliği olmalıdır.

## 2. Developer Platform (SDK ve Eklentiler)
BingehOS'in sadece bir ürün değil, bir "Platform" olmasını sağlayan özelliktir.

### 2.1. Marketplace & Plugins (Eklentiler)
* Sistemin çekirdek koduna dokunmadan üçüncü parti geliştiricilerin özellik ekleyebilmesi gerekir.
* Örneğin: "IoT Klima Bağlantı Eklentisi" veya "SAP Finans Entegrasyon Paketi".
* **Mimarisi:** Sistemde "Hook" veya "Event" noktaları (Örn: İş emri kaydedilmeden hemen önce) tanımlanmalı, eklentiler bu noktalara bağlanarak (Interceptors) araya girebilmelidir.

### 2.2. Public API ve SDK
* Müşterilerin kendi yazılım ekiplerinin entegrasyon yapabilmesi için OpenAPI 3.0 standartlarında, Swagger destekli dökümantasyon portalı (Örn: `developers.bingehos.com`) hazırlanmalıdır.
* Popüler diller (Python, JavaScript, C#) için hazır API Wrapper (SDK) paketleri sağlanmalıdır.

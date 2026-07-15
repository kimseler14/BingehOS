# BingehOS Web

Next.js 14 App Router arayüzü. Geliştirme için:

```bash
npm install
npm run dev
```

API istekleri `/api/*` üzerinden Next.js rewrite ile backend'e iletilir. Varsayılan backend
`http://localhost:8080` adresindedir; farklı bir adres için `API_URL` ortam değişkenini ayarlayın.

```bash
API_URL=http://localhost:8080 npm run dev
```

Üretim derlemesi PWA kurulabilirliği için `manifest.webmanifest` ve bir servis
çalışanı üretir. Servis çalışanı statik dosyaları önbellekten, salt okunur API
yanıtlarını ağdan öncelikli yükler; kimlik doğrulama uçları ve yazma istekleri
önbelleğe alınmaz. Bağlantı yokken `/offline` Türkçe geri dönüş ekranı gösterilir.
Tam çevrimdışı veri senkronizasyonu henüz kapsamda değildir.

Giriş için backend'in seed ettiği tenant ve kullanıcı hesabını kullanın. Yerel API/seed hesabı
ortama göre değişebileceğinden, sabit bir şifre bu dokümana yazılmaz.

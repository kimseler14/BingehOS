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

Giriş için backend'in seed ettiği tenant ve kullanıcı hesabını kullanın. Yerel API/seed hesabı
ortama göre değişebileceğinden, sabit bir şifre bu dokümana yazılmaz.

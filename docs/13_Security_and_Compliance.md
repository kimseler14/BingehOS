# Security & Compliance

## 1. Compliance (Uyumluluk Standartları)
FacilityOS, çok uluslu kurumlar için sertifikasyon süreçlerine uygundur:
* **ISO 55001:** Varlık Yönetimi ve yaşam döngüsü denetimi.
* **ISO 27001:** Bilgi Güvenliği.
* **ISO 41001:** Tesis Yönetimi standartları.
* **ISO 9001, 14001, 45001, FDA, GMP:** Üretim ve kalite yönetim kuralları (Örn: LOTO onayı).

> **Tutarlılık Notu:** Yukarıdaki evrensel standartlar Core kapsamındadır. Bölgesel uyum gereksinimleri (Türkiye'ye özgü KVKK modülü dahil) için bkz. **Doküman 17** — ilgili kapsam ve Turkey plugin sınırları orada tanımlanmıştır. Bu belge (Doküman 13) yalnızca anonimleştirme, audit çatışması ve rıza yönetimi çatısını ele alır; tekrarı önlemek için bölgesel detaylar Doküman 17'ye bırakılmıştır.

## 2. IAM (Yetkilendirme ve Kimlik)
* **RBAC & ABAC:** Roller ve lokasyon bazlı kurallar (Örn: "Sadece İzmir kampüsündeki klimalar").
* **MFA:** Yönetici hesapları için zorunlu 2FA / WebAuthn donanım anahtarları.

## 3. Veri Güvenliği ve Şifreleme (Encryption)
* **TDE Alternatifi:** PostgreSQL'de native TDE (Transparent Data Encryption) yoktur. Uygulamada sunucu diskleri OS seviyesinde (LUKS) şifrelenir. Ekstra hassas kişisel veriler veritabanı içinde `pgcrypto` ile kolon bazlı şifrelenebilir.
* **TLS 1.3:** Ağ içi ve dışı tüm veri iletimi şifrelidir.

## 4. GDPR (KVKK) vs Audit Log Çatışması
* **Sorun:** Audit logları "Immutable" (değiştirilemez) 5 yıl saklanmak zorundadır, ancak GDPR "Unutulma Hakkı" kişisel verilerin silinmesini ister.
* **Çözüm (Data Anonymization / Pseudonymization):** Sistemde yanlış etiketlenen *Crypto-Shredding* (Her kullanıcıya özel DEK anahtarı atayıp, silinince anahtarı yok etme) mimarisine girmeden, maliyet/performans açısından daha verimli olan **Anonimleştirme** yöntemi seçilmiştir:
  * Sistemdeki hiçbir Audit loguna kullanıcı adı/eposta (PII) basılmaz; sadece `User UUID` saklanır.
  * Bir kullanıcı GDPR/KVKK talebiyle silindiğinde, `users` tablosundaki orijinal kaydı silinmez ancak tüm kişisel verileri maskelenir (`name: "deleted_user_123"`, `email: "anon_123@facilityos.com"`).
  * Sonuç olarak; Audit logları (veri bütünlüğü) bozulmaz, ancak UUID artık hiçbir gerçek insana bağlanamayacağı için yasal uyumluluk (Unutulma Hakkı) sağlanmış olur.

## 5. KVKK Rıza Yönetimi (KvkkConsent)
Türkiye Cumhuriyeti KVKK (6698 sayılı Kişisel Verilerin Korunması Kanunu) kapsamında **açık rıza** ve **aydınlatma yükümlülüğü** gereksinimleri, Compliance context'indeki **`KvkkConsent` aggregate'i** ile yönetilir (bölgesel kapsam ve Turkey plugin sınırları için bkz. Doküman 17 §3.1).
* **Aydınlatma Metni Onayı (Explicit Consent):** Kullanıcıya sunulan KVKK Aydınlatma Metni, sürüm (version) bazında saklanır ve kullanıcının ilgili sürümü kabul ettiği açık şekilde kaydedilir.
* **Açık Rıza Loglaması:** Her rıza (onay/ret/geri çekme) olayı; kimin, hangi veri işleme amacı için, hangi tarihte ve hangi kanıt (IP, imza, onay metni hash'i) ile rıza verdiğini içeren **immutable log** olarak `KvkkConsent` aggregate'inde tutulur.
* **Saklama Süresi (Retention) Yönetimi:** Her rıza ve ilişkili kişisel veri için yasal saklama süresi tanımlanabilir; süre dolduğunda verinin otomatik olarak anonimleştirilmesi veya silinmesi (bkz. §4 Crypto-Shredding/Anonimleştirme yaklaşımı) planlanır.
* **Geri Çekilebilirlik:** Kullanıcı açık rızasını dilediği zaman geri çekebilir; geri çekme, mevcut ve gelecekteki veri işleme faaliyetlerini `KvkkConsent` durumu üzerinden sınırlandırır.

# Software Requirements Specification (SRS) - Core Platform

## 1. Giriş
Core Platform, uygulamanın üzerinde koşacağı güvenli, ölçeklenebilir ve çok kiracılı (multi-tenant) altyapıyı sağlar.

## 2. Multi-Tenant Yönetimi
* **Veri İzolasyonu:** PostgreSQL veritabanında her tablo `tenant_id` barındırmalı ve Row-Level Security (RLS) politikaları ile veriler kesin izole edilmelidir.
* **Lisans:** Her tenant'ın aktif kullanıcı veya metrekare kotaları yönetilebilmelidir.

## 3. Identity and Access Management (IAM)
* **Authentication:** Email/Şifre, Social Login, Kurumsal SSO (SAML 2.0, OAuth 2.0, Active Directory/LDAP), ve Passkey/WebAuthn.
* **Authorization:** Rol bazlı (RBAC) ve nitelik bazlı (ABAC) yetkilendirme.

## 4. Organizasyon ve Takımlar
* Hiyerarşik Organizasyon (Campus -> Building -> Department) ve Takım bazlı atamalar.

## 5. Audit ve Loglama
* **Audit Trail:** Tüm işlemler (CREATE/UPDATE/DELETE) immutable (değiştirilemez) olarak saklanır.
* **Sistem Logları:** Hata ve operasyonel loglar merkezi olarak **Grafana Loki** üzerinde saklanmalıdır.

## 6. Workflow ve Otomasyon Motoru
* Kural Motoru (Rule Engine), Trigger'lar ve Aksiyonlar (Email, Webhook).

## 7. Bildirim Altyapısı
* Email, SMS, Push Notification ve In-App bildirim.

## 8. Dosya ve Medya Yönetimi
* Sisteme yüklenen resim, belge ve PDF dosyaları S3 uyumlu **MinIO** üzerinde tutulmalıdır.

## 9. API ve Entegrasyonlar
* **API Gateway:** Dış dünya ile iletişim REST JSON üzerinden sağlanır; Rate Limiting uygulanır.
* **Developer API:** 3. parti geliştiriciler için token bazlı REST API sunulur.
* **Webhooks & Plugins:** Dış sistemlere veri fırlatma ve eklenti yükleme desteği.

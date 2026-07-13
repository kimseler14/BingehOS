# API Specification & OpenAPI

## 1. API Standartları (RESTful JSON)
* BingehOS yalnızca REST tabanlı JSON API sunar (GraphQL projeden çıkarılmıştır).
* Dil Desteği: İsteklerde HTTP header'ı kullanılarak i18n çevirileri sunulur.

## 2. OpenAPI 3.0 Spesifikasyon Örneği
(10 Durumlu State Machine [REJECTED dahil] ve Global `Accept-Language` konfigürasyonu içerir).

```yaml
openapi: 3.0.3
info:
  title: BingehOS API
  version: 1.0.0
servers:
  - url: https://api.bingehos.com/v1
paths:
  /work-orders:
    get:
      summary: İş emirlerini listele
      parameters:
        - $ref: '#/components/parameters/AcceptLanguage'
      responses:
        '200':
          description: Başarılı liste döndürme
    post:
      summary: Yeni iş emri oluştur
      parameters:
        - $ref: '#/components/parameters/AcceptLanguage'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                assetId:
                  type: string
                  format: uuid
                description:
                  type: string
      responses:
        '201':
          description: İş Emri DRAFT statüsünde oluşturuldu
  /work-orders/{id}/status:
    patch:
      summary: İş emri durumunu değiştir
      parameters:
        - $ref: '#/components/parameters/AcceptLanguage'
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                newStatus:
                  type: string
                  enum: [DRAFT, REQUESTED, APPROVED, REJECTED, ASSIGNED, IN_PROGRESS, ON_HOLD, COMPLETED, VERIFIED, CLOSED]
      responses:
        '200':
          description: Başarılı
components:
  parameters:
    AcceptLanguage:
      in: header
      name: Accept-Language
      schema:
        type: string
        default: en-US
      description: Dönen cevabın dil formatı (i18n çevirileri için)
```

## 3. Phase 2 Endpoint Kategorileri (Backlog)
Aşağıdaki route kategorileri, Doküman 07'deki yeni bounded context'ler (HSE, HR & Personnel/SGK, Finance & Tax, Compliance) ve Türkiye'ye özgü hazır iş planı şablonu SEED VERİSİ için Phase 2 kapsamında detaylandırılacaktır. Bu bölüm yalnızca kategori/route listesidir; tam şema (request/response tanımları) henüz belirlenmemiştir. (Not: Job Plan Template mekanizması MVP'dir; bkz. aşağıdaki Not.)

| Route Kategorisi | İlgili Bounded Context | Açıklama |
|---|---|---|
| `/permits` | HSE (PermitToWork, LotoProcedure) | Çalışma izinleri ve LOTO yönetimi |
| `/employees` | HR & Personnel/SGK (Employee, SgkRecord, Subcontractor) | Personel, SGK sicili ve alt işveren yönetimi |
| `/invoices` | Finance & Tax (Invoice, TaxRecord) | e-Fatura/e-Arşiv ve vergi kayıtları |
| `/cost-centers` | Finance & Tax (CostCenter, MonetaryAmount) | Masraf merkezi ve mali birim yönetimi |
| `/kvkk-consents` | Compliance (KvkkConsent) | KVKK açık rıza ve aydınlatma yönetimi |
| `/calibrations` | Compliance (CalibrationRecord) | Kalibrasyon geçerlilik takibi |
| `/compliance-records` | Compliance (ComplianceRecord) | ISO 55001/41001/9001/45001 sertifikasyon kayıtları |

> **Not (Job Plan Template / Standart Job Library):** `/job-plan-templates` endpoint'inin **MEKANİZMASI (genel şablon altyapısı) MVP'dir** ve Doküman 01-16 kapsamında yer alır; bu nedenle yukarıdaki Phase 2 tablosundan çıkarılmıştır. Yalnızca **Türkiye'ye özgü önceden doldurulmuş (pre-seeded) ekipman şablonlarının SEED VERİSİ** (Asansör/Yangın/Jeneratör/HVAC) **Phase 2 / Turkey Compliance Pack (Doküman 17)** kapsamındadır.
>
> Yukarıdaki `/permits`, `/employees`, `/invoices`, `/cost-centers`, `/kvkk-consents`, `/calibrations` ve `/compliance-records` endpoint'leri Phase 2 (Doküman 17) kapsamında tam OpenAPI tanımıyla belirlenecektir; MVP (Doküman 01-16) kapsamı dışındadır.

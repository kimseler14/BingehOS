# BingehOS - Software Architecture Specification

Bu dokümantasyon dizini, BingehOS (Enterprise Facility Operating System) projesinin tüm mimari, tasarım ve gereksinim kararlarını barındıran **Tek Doğruluk Kaynağıdır (Single Source of Truth)**.

Tüm "Acaba?" soruları ortadan kaldırılmış, teknoloji yığını kesinleşmiştir:
* **Mimari:** Modüler Monolit
* **Backend:** C# / .NET 8
* **Veritabanları:** PostgreSQL 16 (Primary), TimescaleDB (Time-Series)
* **Önbellek & Mesajlaşma:** Redis, RabbitMQ
* **Depolama & Loglama:** MinIO, Grafana Loki, Elasticsearch
* **Arayüz:** Next.js (Web), React Native (Mobil)

## 1. Software Requirements Specification (SRS)
- [x] [01_SRS_Core_Platform.md](./01_SRS_Core_Platform.md)
- [x] [02_SRS_Facility_and_Asset.md](./02_SRS_Facility_and_Asset.md)
- [x] [03_SRS_Maintenance_WorkOrder.md](./03_SRS_Maintenance_WorkOrder.md)
- [x] [04_SRS_Inventory_Financial.md](./04_SRS_Inventory_Financial.md)
- [x] [05_SRS_Energy_IoT.md](./05_SRS_Energy_IoT.md)
- [x] [06_SRS_AI_Analytics_DigitalTwin.md](./06_SRS_AI_Analytics_DigitalTwin.md)

## 2. Domain Driven Design (DDD)
- [x] [07_DDD_Architecture.md](./07_DDD_Architecture.md) (Bounded Contexts, Aggregates & Vendor Context)

## 3. Veritabanı ve Veri Modeli
- [x] [08_ER_Diagram_Database.md](./08_ER_Diagram_Database.md) (PostgreSQL / TimescaleDB & ER Diyagramları)

## 4. API Spesifikasyonları
- [x] [09_API_Specification.md](./09_API_Specification.md) (RESTful JSON, OpenAPI)

## 5. Sistem Mimarisi & Dağıtım (Deployment)
- [x] [10_System_Deployment_Architecture.md](./10_System_Deployment_Architecture.md) (Modular Monolith, K8s, Observability)

## 6. UI/UX Tasarım Sistemi
- [x] [11_UI_UX_Design_System.md](./11_UI_UX_Design_System.md) (Standartlar ve Tema)
- [x] [16_UI_Wireframes_and_Component_Tree.md](./16_UI_Wireframes_and_Component_Tree.md) (Web ve Mobil Component Yapıları)

## 7. Developer Platform (SDK & Plugins)
- [x] [12_SDK_and_Plugins.md](./12_SDK_and_Plugins.md) (Marketplace, Eklenti Mimarisi)

## 8. Güvenlik, Test ve Ek Mimariler
- [x] [13_Security_and_Compliance.md](./13_Security_and_Compliance.md) (ISO, GDPR, Audit, RBAC)
- [x] [14_Testing_and_Migration_Strategy.md](./14_Testing_and_Migration_Strategy.md) (Test Piramidi ve Veri Taşıma)
- [x] [15_Localization_Architecture.md](./15_Localization_Architecture.md) (Çoklu Dil ve Timezone)

## 9. Ekstra Özellikler ve Yerel Mevzuat
- [x] [17_Turkey_Compliance_Pack_and_Feature_Backlog.md](./17_Turkey_Compliance_Pack_and_Feature_Backlog.md) (İSG, SGK, E-Devlet, Asansör, ERP) **(PHASE 2 BACKLOG)**

## 10. Sürüm Kapsamı (MVP ve Phase 2)
* **MVP (Minimum Viable Product):** Doküman **01-16**. Çekirdek platform, varlık yönetimi, bakım, finansal envanter, enerji/IoT, AI/Dijital İkiz, DDD, API, dağıtım, UI/UX ve güvenlik/yerelştirme gereksinimlerini kapsar.
* **Phase 2 (Backlog):** Doküman **17** — Turkey Compliance Pack (Marketplace plugin olarak) ve gelişmiş platform özellikleri (Copilot, Digital Twin, Automation Studio, Multi-Tenant, vb.). Core'a gömülmez; Marketplace plugin modeliyle (Doküman 12) sunulur.

## 11. Uygulama Stratejisi (Kodlama Öncesi)
- [x] [18_Implementation_Strategy_and_MVP_Scope.md](./18_Implementation_Strategy_and_MVP_Scope.md) (Solution yapısı, 10 öneri, MVP kesimi, ilk sprint hedefi)


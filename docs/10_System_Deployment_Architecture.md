# System Architecture & Deployment

## 1. Mimari Karar: Strict Modular Monolith (.NET 8)
Proje kesin olarak **Modüler Monolit** mimarisinde, C# ve .NET 8 kullanılarak geliştirilecektir. 
Çelişkili "mikroservis" kavramları terk edilmiştir. Sistem fiziksel olarak tek bir işlem (Single Process) olarak ayağa kalkar, ancak mantıksal olarak Domain bazlı (örn: `BingehOS.Modules.Asset`, `BingehOS.Modules.WorkOrder`) projelere ayrılır.

Modüller birbirlerinin veritabanı tablolarına doğrudan `JOIN` atamazlar. Veri iletişimi C# `MediatR` kütüphanesi üzerinden In-Memory Domain Event'ler ile sağlanır.

## 2. Teknoloji Yığını (Tech Stack)
* **Backend:** C# (.NET 8), Entity Framework Core
* **Database:** PostgreSQL 16
* **Time-Series:** TimescaleDB (Postgres extension)
* **Cache & Locks:** Şu anda runtime cache/lock servisi provision edilmemiştir
* **Message Broker:** RabbitMQ (Sistem dışı entegrasyonlar veya arka plan worker'ları için)
* **Storage:** MinIO
* **Search:** Elasticsearch
* **Frontend Web:** Next.js (TypeScript, TailwindCSS)
* **Frontend Mobile:** React Native, WatermelonDB (Offline Sync)

## 3. Gözlemlenebilirlik (Observability)
* **Tracing & Metrics:** OpenTelemetry.
* **Log Aggregation:** Grafana Loki.
* **Dashboard:** Grafana. (Monolit uygulamanın içindeki her modülün bellek tüketimi ve hata logları Loki/Otel ile tek merkezde izlenir).

# Software Requirements Specification (SRS) - Energy & IoT Modules

## 1. Giriş
Enerji ve IoT modülleri, tesisten anlık veri toplamayı sağlar.

## 2. IoT Platform Modülü
### 2.1. Protokol Destekleri
* BACnet/IP, Modbus TCP/RTU, OPC-UA, MQTT, BLE, LoRaWAN.

### 2.2. Veri İşleme (Telemetry)
* Cihazlardan saniyede binlerce veri noktası akar. Bu veriler (sıcaklık, basınç) zaman serisi formatında **TimescaleDB** üzerinde saklanmalıdır.
* Gelen veriler `Asset`'ler ile eşleştirilir.

## 3. Energy (Enerji Yönetimi) Modülü
### 3.1. Kaynak Tüketimi
* Elektrik, Su, Gaz vb. izleme.

### 3.2. ESG ve Karbon Ayak İzi
* ISO 50001 standartlarında karbon emisyon hesaplaması.

### 3.3. Enerji Optimizasyonu
* **Peak Demand (Pik Talep):** Tesisin enerji talebi eşiği aştığında, Otomasyon Motoru tetiklenerek belirli cihazlara `Kapat/Kıs` sinyali yollar.

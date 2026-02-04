Şimdi tüm sistemin kaynaklarını net bir şekilde özetleyeyim:

---

## **🗺️ NexusSentinel Sistem Kaynakları (Resources)**

Aspire Dashboard'daki Graph'a göre sistemde **6 resource** var. İşte her birinin görevi:

---

### **1. 🐘 postgres (`localhost:55912`)**

| **Özellik** | **Değer** |
| --- | --- |
| **Tür** | PostgreSQL Veritabanı Sunucusu (Docker Container) |
| **Görevi** | Tüm verilerin kalıcı olarak saklandığı ana veritabanı sunucusu |
| **Nasıl Eklendi** | `builder.AddPostgres("postgres").WithDataVolume()` |

---

### **2. 🗄️ sqldb (No endpoints)**

| **Özellik** | **Değer** |
| --- | --- |
| **Tür** | Logical Database (PostgreSQL içindeki bir veritabanı) |
| **Görevi** | `Telemetry` tablolarının bulunduğu spesifik veritabanı |
| **Tablolar** | `Telemetries` (DeviceId, Temperature, Humidity, Timestamp) |
| **Nasıl Eklendi** | `postgres.AddDatabase("sqldb")` |

> *⚠️ **Not:** Bu bir "endpoint" değil, sadece postgres içindeki bir database ismi.*
> 

---

### **3. 🔧 pgadmin (`localhost:55911`)**

| **Özellik** | **Değer** |
| --- | --- |
| **Tür** | Web-based PostgreSQL Yönetim Aracı (Docker Container) |
| **Görevi** | Veritabanını görsel olarak yönetmek, SQL sorguları çalıştırmak |
| **Nasıl Eklendi** | `.WithPgAdmin()` |

---

### **4. 🌡️ simulator (No endpoints - Background Worker)**

| **Özellik** | **Değer** |
| --- | --- |
| **Tür** | .NET Background Service (Worker Service) |
| **Görevi** | **IoT Sensör Simülatörü** - Sahte hava durumu verileri üretir |
| **Davranış** | Her **5 saniyede** bir 

`apiservice`'e telemetri gönderir |
| **Ürettiği Veri** | `Temperature: 20-30°C`, 

`Humidity: 40-60%` |
| **Device ID** | `THERMO-001` |

```

Simulator → POST /telemetry → ApiService → PostgreSQL

```

---

### **5. ⚙️ apiservice (`localhost:7427`)**

| **Özellik** | **Değer** |
| --- | --- |
| **Tür** | ASP.NET Core Minimal API |
| **Görevi** | **Backend / REST API** - Veri işleme merkezi |
| **Endpoints** |  |
| `GET /weatherforecast` | Rastgele hava tahminleri (örnek endpoint) |
| `POST /telemetry` | Simulator'dan gelen veriyi veritabanına yazar |
| `GET /telemetry` | Son 50 telemetri kaydını döner |
| **Bağımlılıklar** | `sqldb` (PostgreSQL) |

---

### **6. 🖥️ webfrontend (`localhost:7108`)**

| **Özellik** | **Değer** |
| --- | --- |
| **Tür** | Blazor Web App (Interactive Server) |
| **Görevi** | **Kullanıcı Arayüzü** - Verileri görselleştirir |
| **Özellikler** | Output Caching, Razor Components |
| **Bağımlılıklar** | `apiservice` (veri çekmek için) |

---

## **📊 Veri Akış Özeti**

```

┌─────────────┐      HTTP POST       ┌─────────────┐      EF Core      ┌─────────────┐
│  Simulator  │ ──────────────────▶  │  ApiService │ ────────────────▶ │  PostgreSQL │
│  (Worker)   │     /telemetry       │   (API)     │                   │   (sqldb)   │
│             │                      │             │                   │             │
│ Her 5 sn    │                      │ Veriyi      │                   │ Kalıcı      │
│ sahte veri  │                      │ işler       │                   │ saklama     │
└─────────────┘                      └──────┬──────┘                   └─────────────┘
                                            │
                                            │ HTTP GET /telemetry
                                            ▼
                                     ┌─────────────┐
                                     │ WebFrontend │
                                     │  (Blazor)   │
                                     │             │
                                     │ Kullanıcıya │
                                     │ gösterir    │
                                     └─────────────┘

```

---

## **🎯 Mevcut Durumda Eksik Olan Şeyler**

| **Eksik** | **Açıklama** |
| --- | --- |
| **Real-time UI Update** | WebFrontend şu an polling yapmıyor veya SignalR yok |
| **AI/Semantic Kernel** | Dokümantasyonda var ama kodda henüz entegre edilmemiş |
| **Anomaly Detection** | Sıcaklık/nem eşik kontrolü yok |
| **Caching** | Redis yok, her istek DB'ye gidiyor |
| **Auth** | Kimse login olmuyor, API açık |
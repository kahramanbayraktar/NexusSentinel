# Project Architecture: .NET Aspire & Integration

**NexusSentinel** is built using **.NET Aspire**, which is an opinionated, cloud-ready stack for building observable, production-ready, distributed applications.

## 🏗️ The Orchestration Layer (`AppHost`)
The `AppHost` project is the "brain" of our local development environment. 
- It manages the lifecycle of the **ApiService**, **Web** frontend, and **PostgreSQL** database.
- Instead of manually starting 3-4 projects, we start one, and Aspire handles the networking and discovery.

## 🔗 Service Discovery
In modern microservices, hardcoding URLs (like `http://localhost:5001`) is a bad practice.
- **Aspire Solution:** We use names like `"apiservice"`. Aspire's service discovery mechanism automatically maps these names to the correct internal URLs during development and in the cloud.

## 🛠️ Service Defaults
The `ServiceDefaults` project is a shared project that configures:
- **Telemetry (OpenTelemetry):** Distributed tracing for debugging.
- **Health Checks:** Ensuring services are alive.
- **Resilience:** Handling temporary network glitches.

## 🗄️ Database Integration (Npgsql)
We chose **PostgreSQL** with the **Npgsql** EF Core provider.
- **Challenge:** Configuring a database locally usually involves Docker setup and connection strings.
- **Aspire Advantage:** We added the database to the builder:
  ```csharp
  var postgres = builder.AddPostgreSQL("postgres");
  var sqldb = postgres.AddDatabase("sqldb");
  ```
  Aspire handles the container orchestration and provides the connection string to the ApiService automatically via "Resource Binding."

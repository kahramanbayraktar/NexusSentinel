# .NET & C# Technical Learning Guide

This guide documents the coding patterns, CLI commands, and best practices encountered during the development of NexusSentinel.

---

## 🏗️ C# Data Structures: Records

In this project, we use `record` for our telemetry data.

### Record vs. Class
- **Records** are primarily used for "data-centric" types. They provide built-in **value-based equality** (two records are equal if their properties are equal, even if they are different objects in memory).
- **Immutability:** By default, properties in a primary constructor are `init`-only, making the data thread-safe and unchangeable after creation.

### Primary Constructors
When you define a record like this:
```csharp
public record TelemetryData(string DeviceId, double Temperature, ...);
```
C# automatically creates a constructor. You must pass all arguments during instantiation:
```csharp
var data = new TelemetryData("ID-01", 25.5, ...); // Correct
```
Using the object initializer `{ DeviceId = "..." }` will fail unless you explicitly define a parameterless constructor or change the record structure.

---

## 🛠️ CLI Commands (The Essentials)

- **`dotnet build`**: Compiles the entire solution. Use this to check for syntax errors or reference issues without running the app.
- **`dotnet run --project <Path>`**: Builds and immediately starts the specified project. When running the `AppHost`, it starts the entire Aspire ecosystem.
- **`dotnet watch`**: (Bonus) Automatically rebuilds and restarts the app whenever you save a file.
- **`dotnet new <Template>`**: Creates a new project from a template (e.g., `worker`, `classlib`, `web`).

---

## 🪵 Logging Best Practices

### `if (logger.IsEnabled(LogLevel.Information))`
Why do we use this?
1. **Performance:** It prevents the generation of log strings and the evaluation of parameters if the current logging level is set higher (e.g., to `Warning`).
2. **Memory Allocation:** Avoids creating temporary objects on the heap that would otherwise need to be garbage collected.
3. **Clean Logs:** Ensures that high-frequency background tasks don't pollute the logs unless explicitly requested.

---

## 📡 Networking in .NET Aspire

### Service Discovery
Instead of using hardcoded IPs like `https://localhost:5001`, we use service names:
```csharp
client.BaseAddress = new("https://apiservice");
```
- **How it works:** .NET Aspire's `AppHost` acts as a local DNS. It knows which port `apiservice` is running on and automatically maps the request.
- **Benefit:** If the port changes or if you move to a containerized environment (Docker/Kubernetes), the code remains exactly the same.

---

## 🔐 Aspire Dashboard Authentication

When you first open the .NET Aspire Dashboard, you will be prompted for a **Login Token**. This is a security feature to ensure only authorized users can view the system's traces and logs.

### 1. How to find the Token
Look at your terminal/console output after running `dotnet run`. You will see a log entry similar to:
`Login to the dashboard at https://localhost:17267 with the token: abcdef123456...`

### 2. Why it exists
The dashboard contains sensitive information, including request headers, environment variables, and distributed traces. The token ensures this data isn't exposed to everyone on the network during development.

---

## 🔄 Background Processing

### `BackgroundService` vs. `Worker`
- **`BackgroundService`**: A base class in .NET used for long-running tasks.
- **`ExecuteAsync`**: The core method you override. It contains the `while (!stoppingToken.IsCancellationRequested)` loop which ensures the task stops gracefully when the application shuts down.
- **`stoppingToken`**: Always pass this token to `Task.Delay` or Async methods to allow for immediate shutdown without hanging.

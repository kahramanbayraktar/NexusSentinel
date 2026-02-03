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

---

## ⚡ HttpClient & BackgroundService Issues

### ❌ The Problem: Typed Clients vs. BackgroundService
When using `AddHttpClient<Worker>(...)` (Typed Client) alongside `AddHostedService<Worker>()`, a common issue arises where the `BackgroundService` instance created by the host might not correctly receive the configured `HttpClient`. This often results in:
- `InvalidOperationException`: "An invalid request URI was provided."
- `BaseAddress` being null inside the Worker as the injection chain gets broken.

### ✅ The Solution: IHttpClientFactory (Named Clients)
Using `IHttpClientFactory` with a **Named Client** is the most robust approach for long-running services:

1. **In `Program.cs`**:
```csharp
builder.Services.AddHttpClient("apiservice", client => {
    client.BaseAddress = new Uri("http://apiservice/");
});
```

2. **In `Worker.cs`**:
```csharp
public class Worker(IHttpClientFactory httpClientFactory) : BackgroundService {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        var client = httpClientFactory.CreateClient("apiservice");
        // Use client...
    }
}
```

This pattern ensures that the `HttpClient` is always created with the correct configuration from the central factory, regardless of the `Worker`'s lifecycle.
---

## 🌐 Real-Time Communication & Microservices Patterns

Modern distributed systems need to move data efficiently between services and to the end-user. We've explored three main patterns:

### 1. Polling: The "Passive" Approach
In polling, the client (e.g., Web App) periodically asks the server (e.g., API) "Is there any new data?".

*   **Mechanism:** Client-driven via a timer (e.g., every 5 seconds).
*   **The "Double Burden" (Scale Issue):**
    *   **Redundant Requests:** If data hasn't changed, the API still has to process the request, check the database, and return an empty or duplicate response. At scale (10,000+ users), this wastes CPU/RAM.
    *   **Network Overhead (Headers):** Every HTTP request carries heavy headers (User-Agent, Cookies, JWT Tokens). Often, the "envelope" (Headers) is 2KB while the "letter" (Data) is only 40 bytes.
    *   **Latency:** Data is only as fresh as the last poll. If an event happens right after a poll, it sits invisible for almost the entire interval.

### 2. SignalR (Push): The "Reactive" Approach
SignalR allows the server to proactively "push" data to the client as soon as it happens.

*   **Mechanism:** Uses **WebSockets** for a persistent, bi-directional "highway".
*   **Benefits:**
    *   **Efficiency:** Header overhead is only paid once (during the initial handshake). After that, only raw data frames are sent.
    *   **Immediacy:** Zero-latency delivery. As soon as the API receives a sensor reading, it "flicks" it to all connected dashboards.
    *   **Statelessness vs Persistence:** Unlike standard HTTP (stateless), SignalR maintains a "stateful" connection, making it feel alive.

### 3. gRPC: High-Performance Service-to-Service
Where SignalR is king for Server-to-Client (Browser), gRPC is the gold standard for Service-to-Service communication.

*   **Format:** Uses **Protocol Buffers (Protobuf)**, a binary format, instead of text-based JSON.
*   **Performance:** Binary data is much smaller and faster to serialize/deserialize than text.
*   **Strong Typing:** Contracts are defined in `.proto` files, ensuring both services speak the exact same language (Contract-First approach).
*   **HTTP/2:** Takes advantage of advanced features like multiplexing several requests over a single connection.

### 📊 Tactical Comparison

| Pattern | Best For | Transport | Data Format | Overhead |
| :--- | :--- | :--- | :--- | :--- |
| **Polling** | Simple apps, low traffic | HTTP | JSON/XML | High (Headers per req) |
| **SignalR** | **Client UI updates**, Chat, Dashboards | WebSocket | JSON/Binary | Low (After handshake) |
| **gRPC** | **Internal Microservices**, High throughput | HTTP/2 | Protobuf (Binary) | Minimal |

---

## 🔐 The "Header" Burden & Session Management

We've discussed how headers impact high-scale systems:
1.  **JWT (JSON Web Token):** Often stored in the `Authorization` header. In polling, this large encrypted string is sent every second.
2.  **Cookies (Session/Analytics):** Tarayıcıdaki `_ga` (Google Analytics) veya `sessionid` çerezleri her istekle beraber otomatik gönderilir. Bu "gereksiz yük" (overhead), polling sıklığı arttıkça ağ trafiğini şişirir.
3.  **Stateless API:** High-scale sistemlerde API'ler genellikle "Session" tutmaz. Bunun yerine her isteği kendi içinde doğrular (JWT). Bu, sunucu hafızasını rahatlatsa da istemcinin her seferinde kimliğini (Token) kanıtlaması (Header yükü) maliyetini doğurur.

# NexusSentinel: Technical Concepts & Technology Deep Dive

This document provides a detailed explanation of the core technologies and communication protocols used in the NexusSentinel project.

---

## 🏎️ gRPC (Google Remote Procedure Call)

NexusSentinel leverages gRPC for high-performance service-to-service communication, especially for observability and real-time data streaming.

### Why gRPC?
- **Binary Serialization (Protobuf):** Unlike REST which uses human-readable JSON, gRPC uses **Protocol Buffers**. This results in significantly smaller payloads and faster serialization/deserialization, saving CPU cycles.
- **HTTP/2 Power:** It utilizes HTTP/2 features like **multiplexing** (multiple requests over a single connection) and header compression.
- **Streaming Capabilities:** 
    - *Server Streaming:* Ideal for AI agents returning long responses word-by-word.
    - *Client Streaming:* Perfect for IoT devices pushing high-frequency telemetry.
    - *Bi-directional Streaming:* Allows simultaneous data exchange between services.
- **Contract-First:** API definitions are strictly enforced via `.proto` files, reducing runtime errors and ensuring type safety across services.

---

## 🛡️ HTTP vs. HTTPS

In the .NET Aspire dashboard, you will notice both `http://` and `https://` endpoints. 

### Key Differences in NexusSentinel:
| Feature | HTTP | HTTPS (Recommended) |
| :--- | :--- | :--- |
| **Security** | Plain text (Vulnerable to sniffing) | Encrypted via SSL/TLS (Secure) |
| **Trust** | No identity verification | Server identity verified via certificates |
| **gRPC Support** | Limited / Not recommended | Native and high-performance |
| **Use Case** | Local legacy debugging | Default for all modern service communication |

*Note: For local development, we use `dotnet dev-certs https --trust` to ensure our local environment mirrors a secure production setup.*

---

## 📊 .NET Aspire Dashboard

The Aspire Dashboard acts as the **Mission Control** for the entire distributed system.

### Core Sections:
1. **Resources:** A live list of all running projects (APIs, Frontends, Worker Services). It provides one-click access to endpoints.
2. **Console Logs:** Real-time access to the standard output (`stdout`) of any running service.
3. **Structured Logs:** A powerful filterable view of logs across the entire ecosystem, allowing you to track specific events across service boundaries.
4. **Distributed Tracing:** Allows you to "see through" the system. You can trace a single request as it travels from the Blazor UI to the ApiService and finally to the Database, visualizing latencies for each step.
5. **Metrics:** Real-time performance monitoring (CPU, RAM, Request Rates) using OpenTelemetry standards.

---

## 🏗️ Cloud-Native Orchestration

By using **.NET Aspire**, we avoid the "Spaghetti configuration" typical in microservices:
- **Service Discovery:** Services find each other using logical names (e.g., `http://apiservice`) instead of hardcoded IP addresses or ports.
- **Environment Management:** Connection strings and environment variables are automatically injected into the correct containers/projects during startup.
- **Standardized Foundation:** Every service inherits the same resilience, health check, and observability policies via the `ServiceDefaults` project.

---

## 🧠 AI Integration (Semantic Kernel)

**Semantic Kernel** is the framework that allows our C# code to interact with Large Language Models (LLMs).
- **Plugins:** Encapsulates C# functions that the AI can call (e.g., "GetSensorHistory", "GenerateAlert").
- **Planners:** The AI looks at the available plugins and creates a plan to solve a complex user request or analyze a system anomaly.
- **Orchestration:** It bridges the gap between raw telemetry and natural language intelligence.

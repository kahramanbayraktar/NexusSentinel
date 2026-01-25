# NexusSentinel: Intelligent IoT Sentinel

![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)
![Aspire](https://img.shields.io/badge/Aspire-9.0-purple.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

NexusSentinel is a bleeding-edge, cloud-native autonomous monitoring system. It leverages the power of **.NET 9**, **.NET Aspire**, and **Semantic Kernel** to transform raw IoT telemetry into actionable AI-driven intelligence.

## 🌟 Key Features
- **Intelligent Monitoring:** Uses AI Agents (Semantic Kernel) to analyze sensor patterns and provide natural language insights.
- **Cloud-Native Orchestration:** Fully managed service lifecycle and observability via .NET Aspire.
- **Real-time Dashboard:** Reactive UI built with Blazor, showing live metrics and AI-generated health reports.
- **Scalable Architecture:** Distributed microservices pattern ready for containerization.

## 🏗 Project Structure
- `NexusSentinel.AppHost`: The orchestration entry point.
- `NexusSentinel.ApiService`: The backend engine with AI integrations.
- `NexusSentinel.Web`: The Blazor-based visualization dashboard.
- `NexusSentinel.ServiceDefaults`: Shared resilience and observability policies.

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [.NET Aspire Workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)
- Optional: Docker Desktop (for containerized resources)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/NexusSentinel.git
   cd NexusSentinel
   ```
2. Run the application:
   ```bash
   dotnet run --project NexusSentinel.AppHost
   ```
3. Open the **Aspire Dashboard** link displayed in your terminal to view the services.

## 📖 Documentation
For a detailed look at the technical architecture and design decisions, please refer to the [Architecture Guide](./docs/ARCHITECTURE.md).

## 📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

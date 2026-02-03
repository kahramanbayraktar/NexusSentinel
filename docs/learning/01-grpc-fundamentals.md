# gRPC Implementation & Fundamentals

## 🚀 Why gRPC?
In the **NexusSentinel** project, we needed a way for the **Simulator** (IoT Device) to send rapid, frequent telemetry data to the **ApiService**. 

### Decisions & Trade-offs:
- **Performance:** Unlike REST/JSON, gRPC uses **Protocol Buffers (Protobuf)**, which is a binary serialization format. This means smaller payloads and faster processing.
- **Contract-First:** We define the data structure in a `.proto` file. This acts as a single source of truth for both the client (Simulator) and the server (ApiService).
- **Type Safety:** The C# classes are automatically generated from the proto definition, preventing manual mapping errors.

## 🛠️ Lessons Learned: The Implementation Process

### 1. The Proto Definition (`telemetry.proto`)
We defined a `TelemetryService` with a `SendTelemetry` method.
```protobuf
service TelemetryService {
    rpc SendTelemetry (TelemetryRequest) returns (TelemetryResponse);
}
```
**Challenge:** Protobuf is very strict about syntax. Missing a semicolon `;` at the end of `syntax = "proto3";` or a typo like `strign` instead of `string` will break the code generation.

### 2. The MSBuild Magic (`.csproj`)
To trigger code generation, we added a `<Protobuf />` item to the `Shared` project.
```xml
<ItemGroup>
  <Protobuf Include="Protos\telemetry.proto" GrpcServices="Both" />
</ItemGroup>
```
**Challenge encountered:** I initially wrote `GrpServices` (missing the 'c'). This resulted in the build succeeding but **no C# classes being generated**. 
**Lesson:** Always double-check MSBuild attribute names; they are case-sensitive and specific.

### 3. Server-Side Implementation
We inherited from the generated `TelemetryService.TelemetryServiceBase` to implement the logic.
- **Key Knowledge:** gRPC methods in C# are almost always `async` and use `ServerCallContext` for request metadata.

## 🔍 Lower-Level Details
- **HTTP/2:** gRPC requires HTTP/2. This allows for features like multiplexing (sending multiple requests over a single connection) and server-side streaming, which we might use later for real-time alerts.

## 🛡️ Dependency Management & NuGet Challenges

### 4. The "Package Downgrade" Error (NU1605)
During the renaming of the Simulator to **IoTSimulator**, we encountered a critical NuGet error:
`Warning As Error: Detected package downgrade: Grpc.Net.Client from 2.76.0 to 2.65.0.`

**The Cause:**
- `NexusSentinel.Shared` was referencing `Grpc.Net.Client (2.76.0)`.
- `NexusSentinel.IoTSimulator` was manually added with an older version `(2.65.0)`.
- Since `IoTSimulator` depends on `Shared`, NuGet detected a conflict where a project was trying to use a lower version than its dependency required.

**The Solution:**
Always align NuGet package versions across the entire solution. We resolved this by updating all gRPC-related packages to the same stable version (`2.76.0`) in all projects.

### 5. Missing Client Factory
Another challenge was the error: `'IServiceCollection' does not contain a definition for 'AddGrpcClient'`.
**Lesson:** To use the `AddGrpcClient` extension method for Dependency Injection, you must install the specific `Grpc.Net.ClientFactory` NuGet package, not just the base client.

## ⌛ Handling Time: `google.protobuf.Timestamp`

### 6. Why we use standard Timestamps
IoT sensor data is useless without a timestamp. However, DateTime in C# isn't directly compatible with gRPC.

- **The Unused Import Lesson:** We initially imported `google/protobuf/timestamp.proto` but didn't use it in a field. The compiler warned us. Our first instinct was to delete it to "clean the warnings," but the correct engineering decision was to implement the field in the message.
- **Mapping:** We learned to map between .NET `DateTime` and Protobuf `Timestamp`.

**C# Mapping Code:**
- **Sender (Simulator):** `Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dateTime)`
- **Receiver (ApiService):** `request.Timestamp.ToDateTime()`

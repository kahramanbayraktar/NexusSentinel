namespace NexusSentinel.Shared;

public record TelemetryData(
    string DeviceId,
    double Temperature,
    double Humidity,
    DateTime Timestamp
);

using System;
using System.Reflection.Emit;
using NexusSentinel.Shared;

namespace NexusSentinel.Simulator;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    private readonly Random _random = new();
    private string _deviceId = "THERMO-001";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("IoT Simulator is starting for device: {DeviceId}", _deviceId);

        while (!stoppingToken.IsCancellationRequested)
        {
            var telemetry = new TelemetryData(
                DeviceId: _deviceId,
                Temperature: Math.Round(20 + (_random.NextDouble() * 10), 2), // 20-30
                Humidity: Math.Round(40 + (_random.NextDouble() * 20), 2), // 40-60
                Timestamp: DateTime.UtcNow
            );

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Generating Telemetry Data: {Temp}°C, {Humidity}% Hum",
                    telemetry.Temperature,
                    telemetry.Humidity
                );
            }

            // TODO: Send the data to API

            await Task.Delay(2000, stoppingToken);
        }
    }
}

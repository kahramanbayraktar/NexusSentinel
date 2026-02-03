using System;
using System.Net.Http.Json;
using System.Reflection.Emit;
using NexusSentinel.Shared;
using NexusSentinel.Shared.Protos;

namespace NexusSentinel.IoTSimulator;

// We inject IHttpClientFactory instead of HttpClient
public class Worker(ILogger<Worker> logger, TelemetryService.TelemetryServiceClient grpcClient)
    : BackgroundService
{
    private readonly Random _random = new();
    private string _deviceId = "THERMO-001";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("IoT Simulator is starting for device: {DeviceId}", _deviceId);

        // We create the client by name through the factory
        // var httpClient = httpClientFactory.CreateClient("apiservice");

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

            try
            {
                // The httpClient now knows the BaseAddress is "http://apiservice/"
                // var response = await httpClient.PostAsJsonAsync(
                //     "telemetry",
                //     telemetry,
                //     stoppingToken
                // );

                var request = new TelemetryRequest
                {
                    DeviceId = _deviceId,
                    Temperature = telemetry.Temperature,
                    Humidity = telemetry.Humidity,
                    Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(
                        telemetry.Timestamp
                    ),
                };
                var response = await grpcClient.SendTelemetryAsync(request);

                // if (response.IsSuccessStatusCode)
                if (response.Success)
                {
                    logger.LogInformation(
                        "Telemetry sent: {Device} - Temp: {Temp}°C, {Humidity}% Hum",
                        telemetry.DeviceId,
                        telemetry.Temperature,
                        telemetry.Humidity
                    );
                }
                else
                {
                    // logger.LogWarning(
                    //     "Failed to send telemetry. Status: {Status}",
                    //     response.StatusCode
                    // );
                    logger.LogWarning("Failed to send telemetry");
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Failed to send telemetry");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}

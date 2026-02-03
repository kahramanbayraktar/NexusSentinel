using Grpc.Core;
using NexusSentinel.Shared.Protos;

namespace NexusSentinel.ApiService.Services;

public class TelemetryGrpcService : TelemetryService.TelemetryServiceBase
{
    private readonly ILogger<TelemetryGrpcService> _logger;

    public TelemetryGrpcService(ILogger<TelemetryGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<TelemetryResponse> SendTelemetry(
        TelemetryRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation(
            "Received gRPC telemetry: Device={DeviceId}, Temp={Temp}, Hum={Hum}, Time={Timestamp}",
            request.DeviceId,
            request.Temperature,
            request.Humidity,
            request.Timestamp.ToDateTime()
        );

        return Task.FromResult(
            new TelemetryResponse
            {
                Success = true,
                Message = "Data received successfully via gRPC!",
            }
        );
    }
}

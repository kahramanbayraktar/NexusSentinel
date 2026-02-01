namespace NexusSentinel.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(
        int maxItems = 10,
        CancellationToken cancellationToken = default
    )
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (
            var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>(
                "/weatherforecast",
                cancellationToken
            )
        )
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }

    public async Task<TelemetryData[]> GetTelemetryAsync(
        CancellationToken cancellationToken = default
    )
    {
        var telemetryList = await httpClient.GetFromJsonAsync<TelemetryData[]>(
            "/telemetry",
            cancellationToken
        );

        return telemetryList ?? [];
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// ============================================
// TELEMETRY API
// ============================================
/// <summary>
/// Represents telemetry data from IoT sensors
/// </summary>
/// <param name="Id">Database primary key</param>
/// <param name="DeviceId">Unique identifier of the IoT device</param>
/// <param name="Temperature">Temperature reading in Celsius</param>
/// <param name="Humidity">Humidity percentage (0-100)</param>
/// <param name="Timestamp">When the reading was taken (UTC)</param>
public record TelemetryData(
    int Id,
    string DeviceId,
    double Temperature,
    double Humidity,
    DateTime Timestamp
);

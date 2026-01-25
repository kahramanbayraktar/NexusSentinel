namespace NexusSentinel.ApiService.Data;

public class Telemetry
{
    public int Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public DateTime Timestamp { get; set; }
}

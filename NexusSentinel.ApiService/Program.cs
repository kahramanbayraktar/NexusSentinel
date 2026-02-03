using Microsoft.EntityFrameworkCore;
using NexusSentinel.ApiService.Data;
using NexusSentinel.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC services
builder.Services.AddGrpc();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("sqldb");

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Add gRPC service
app.MapGrpcService<TelemetryGrpcService>();

// Add a simple HTTP endpoint for testing
app.MapGet("/grpc", () => "Communication with gRPC endpoints must be made through gRPC.");

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
    "/weatherforecast",
    () =>
    {
        var forecast = Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    }
);

app.MapPost(
    "/telemetry",
    async (NexusSentinel.Shared.TelemetryData data, AppDbContext db) =>
    {
        var telemetry = new Telemetry
        {
            DeviceId = data.DeviceId,
            Temperature = data.Temperature,
            Humidity = data.Humidity,
            Timestamp = data.Timestamp,
        };

        db.Telemetries.Add(telemetry);
        await db.SaveChangesAsync();

        return Results.Created($"/telemetry/{telemetry.Id}", telemetry);
    }
);

app.MapGet(
    "/telemetry",
    async (AppDbContext db) =>
        await db.Telemetries.OrderByDescending(t => t.Timestamp).Take(50).ToListAsync()
);

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

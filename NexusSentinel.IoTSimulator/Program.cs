using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexusSentinel.IoTSimulator;
using NexusSentinel.Shared;
using NexusSentinel.Shared.Protos;

var builder = Host.CreateApplicationBuilder(args);

// 1. We are adding Aspire service standards (Logging, HealthCheck, etc.)
builder.AddServiceDefaults();

builder.Services.AddGrpcClient<TelemetryService.TelemetryServiceClient>(o =>
{
    o.Address = new Uri("http://apiservice");
});

// 2. We are registering the HttpClient for the Worker.
// The name "apiservice" must be exactly the same as the name we provided in AppHost.
// We define an HttpClient named "apiservice" instead of Worker.
// builder.Services.AddHttpClient(
//     "apiservice",
//     client =>
//     {
//         client.BaseAddress = new Uri("http://apiservice/");
//     }
// );

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

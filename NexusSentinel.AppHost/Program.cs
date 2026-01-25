var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.NexusSentinel_ApiService>("apiservice");

builder.AddProject<Projects.NexusSentinel_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

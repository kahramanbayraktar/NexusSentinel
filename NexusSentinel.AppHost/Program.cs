var builder = DistributedApplication.CreateBuilder(args);

// DB
// AddPostgres("postgres"): Tell Aspire, "I need a PostgreSQL database."
// When you run the application, Aspire automatically launches a PostgreSQL Docker container in the background.
// You don't have to deal with details like username, password, and port.
// .WithDataVolume(): Prevents data from being deleted every time you stop and start the container; it stores the data in a Docker volume.
var postgres = builder.AddPostgres("postgres").WithDataVolume().WithPgAdmin();

// Creates/allocates a special database (logical database) named sqldb within that PostgreSQL server.
var sqldb = postgres.AddDatabase("sqldb");

var apiService = builder
    .AddProject<Projects.NexusSentinel_ApiService>("apiservice")
    // This is the "magic" part.
    // When you add this command, Aspire automatically generates the Connection String needed to connect to sqldb and injects it into the ApiService project as an environment variable.
    // You don't write the connection address on the API side; you just connect to the database using the name "sqldb".
    .WithReference(sqldb);

// ApiService
builder.AddProject<Projects.NexusSentinel_Simulator>("simulator").WithReference(apiService);

// Web
builder
    .AddProject<Projects.NexusSentinel_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

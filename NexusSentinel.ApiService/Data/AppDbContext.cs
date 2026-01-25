using Microsoft.EntityFrameworkCore;

namespace NexusSentinel.ApiService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Telemetry> Telemetries => Set<Telemetry>();
}

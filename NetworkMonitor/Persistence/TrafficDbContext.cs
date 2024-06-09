using Microsoft.EntityFrameworkCore;
using NetworkMonitor.Models;

namespace NetworkMonitor.Persistence;

public sealed class TrafficDbContext : DbContext
{
    public DbSet<NetworkStatistics> NetworkStatistics { get; set; }

    public TrafficDbContext(DbContextOptions<TrafficDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}

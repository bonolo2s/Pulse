using Microsoft.EntityFrameworkCore;

namespace Pulse.Monitoring.DataAccess;

public class MonitoringDbContext : DbContext
{
    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options) : base(options) { }

    public DbSet<MonitoredEndpoint> MonitoredEndpoints { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonitoredEndpoint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(2048);
            entity.Property(e => e.Method).IsRequired().HasMaxLength(10);
            entity.Property(e => e.IntervalSeconds).IsRequired();
            entity.Property(e => e.TimeoutMs).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.UserId);
        });
    }
}
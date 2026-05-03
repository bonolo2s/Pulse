using Microsoft.EntityFrameworkCore;
using Pulse.Observability.Entities;

namespace Pulse.Observability.DataAccess;

public class ObservabilityDbContext : DbContext
{
    public ObservabilityDbContext(DbContextOptions<ObservabilityDbContext> options) : base(options) { }

    public DbSet<CheckResult> CheckResults { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.StatusCode).IsRequired();
            entity.Property(e => e.LatencyMs).IsRequired();
            entity.Property(e => e.CheckedAt).IsRequired();
            entity.HasIndex(e => e.EndpointId);
        });
    }
}
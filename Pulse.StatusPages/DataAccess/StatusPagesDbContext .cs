using Microsoft.EntityFrameworkCore;

namespace Pulse.StatusPages.DataAccess;

public class StatusPagesDbContext : DbContext
{
    public StatusPagesDbContext(DbContextOptions<StatusPagesDbContext> options) : base(options) { }

    public DbSet<StatusPage> StatusPages { get; set; } = null!;
    public DbSet<StatusPageEndpoint> StatusPageEndpoints { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatusPage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
            entity.Property(e => e.IsPublic).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasMany(e => e.Endpoints)
                  .WithOne()
                  .HasForeignKey(e => e.StatusPageId);
        });

        modelBuilder.Entity<StatusPageEndpoint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DisplayOrder).IsRequired();
            entity.HasIndex(e => e.StatusPageId);
            entity.HasIndex(e => e.EndpointId);
        });
    }
}
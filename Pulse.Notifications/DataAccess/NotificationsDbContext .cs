using Microsoft.EntityFrameworkCore;
using Pulse.Notifications.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Pulse.Notifications.DataAccess;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options) { }

    public DbSet<AlertRule> AlertRules { get; set; } = null!;
    public DbSet<AlertLog> AlertLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Channel).IsRequired().HasConversion<string>();
            entity.Property(e => e.Destination).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<AlertLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Channel).IsRequired().HasConversion<string>();
            entity.Property(e => e.Type).IsRequired().HasConversion<string>();
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.SentAt).IsRequired();
            entity.HasIndex(e => e.EndpointId);
            entity.HasIndex(e => e.AlertRuleId);
        });
    }
}
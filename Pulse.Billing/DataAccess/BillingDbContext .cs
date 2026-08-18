using Microsoft.EntityFrameworkCore;
using Pulse.Billing.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Pulse.Billing.DataAccess;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<BillingEvent> BillingEvents { get; set; } = null!;

    public DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Plan)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.EndpointLimit).IsRequired();
            entity.Property(e => e.StartedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.IssuedAt).IsRequired();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.SubscriptionId);
        });

        modelBuilder.Entity<BillingEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaystackEventId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EventType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);
            entity.Property(e => e.Payload).IsRequired();
            entity.Property(e => e.Processed).IsRequired();
            entity.Property(e => e.ReceivedAt).IsRequired();
            entity.HasIndex(e => e.PaystackEventId).IsUnique();
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.Method).IsRequired().HasConversion<string>();
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
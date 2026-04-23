using HandsOn022.Domain;
using Microsoft.EntityFrameworkCore;

namespace HandsOn022.Infrastructure;

// =============================================================================
// Shared DbContext (always compiled)
// =============================================================================

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.CustomerId).IsRequired().HasMaxLength(100);
            b.HasMany(o => o.Items).WithOne().HasForeignKey("OrderId");
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.Type).IsRequired().HasMaxLength(200);
            b.Property(m => m.Payload).IsRequired();
        });
    }
}

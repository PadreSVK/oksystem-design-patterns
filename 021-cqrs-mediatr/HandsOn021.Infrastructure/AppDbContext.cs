using HandsOn021.Domain;
using Microsoft.EntityFrameworkCore;

namespace HandsOn021.Infrastructure;

// =============================================================================
// Shared DbContext (always compiled)
// =============================================================================

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderSummaryView> OrderSummaryViews => Set<OrderSummaryView>();

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

        modelBuilder.Entity<OrderSummaryView>(b =>
        {
            b.HasKey(v => v.OrderId);
        });
    }
}

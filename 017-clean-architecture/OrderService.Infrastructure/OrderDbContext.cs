using Microsoft.EntityFrameworkCore;
using OrderService.Domain;

namespace OrderService.Infrastructure;

// =============================================================================
// Shared DbContext (always compiled)
// =============================================================================

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.CustomerId).IsRequired().HasMaxLength(50);
            b.HasMany(o => o.Lines).WithOne().HasForeignKey("OrderId");
        });

        modelBuilder.Entity<OrderLine>(b =>
        {
            b.HasKey(l => l.Id);
            b.Property(l => l.ProductId).IsRequired().HasMaxLength(50);
        });
    }
}

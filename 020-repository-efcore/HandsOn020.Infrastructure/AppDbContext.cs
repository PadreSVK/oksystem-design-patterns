using HandsOn020.Domain;
using Microsoft.EntityFrameworkCore;

namespace HandsOn020.Infrastructure;

// =============================================================================
// Shared DbContext (always compiled)
// =============================================================================
//
// EF Core's DbContext acts as both Repository (DbSet<T>) and Unit of Work
// (SaveChangesAsync). Here we configure entity mappings including the
// Order → OrderItem relationship with eager-loading via Include.
//
// NOTE: An alternative approach uses OwnsMany for value-object semantics:
//   modelBuilder.Entity<Order>()
//       .OwnsMany(o => o.Items, b => { b.WithOwner(); b.ToTable("OrderItems"); });
// =============================================================================

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

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
    }
}

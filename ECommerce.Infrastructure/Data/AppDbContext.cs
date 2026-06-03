using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public abstract class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        protected AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        // Domain DbSets
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product
            modelBuilder.Entity<Product>(b =>
            {
                b.ToTable("Products");
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).IsRequired().HasMaxLength(200);
                b.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            // Customer
            modelBuilder.Entity<Customer>(b =>
            {
                b.ToTable("Customers");
                b.HasKey(c => c.Id);
                b.Property(c => c.FirstName).IsRequired().HasMaxLength(200);
                b.Property(c => c.Email).IsRequired().HasMaxLength(256);
                b.HasMany(c => c.Orders)
                 .WithOne(o => o.Customer)
                 .HasForeignKey(o => o.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Order
            modelBuilder.Entity<Order>(b =>
            {
                b.ToTable("Orders");
                b.HasKey(o => o.Id);
                b.Property(o => o.OrderDate).IsRequired();
                b.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
                b.HasMany(o => o.Items)
                 .WithOne(oi => oi.Order)
                 .HasForeignKey(oi => oi.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem
            modelBuilder.Entity<OrderItem>(b =>
            {
                b.ToTable("OrderItems");
                b.HasKey(oi => oi.Id);
                b.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
                b.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
                b.Property(oi => oi.Quantity).IsRequired();
            });
        }
    }
}

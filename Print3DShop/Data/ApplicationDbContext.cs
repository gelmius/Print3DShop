using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Print3DShop.Models;

namespace Print3DShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PrintingService> PrintingServices { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<CustomPrintRequest> CustomPrintRequests { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .IsRequired(false);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.PrintingService)
                .WithMany()
                .HasForeignKey(oi => oi.PrintingServiceId)
                .IsRequired(false);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.CustomPrintRequest)
                .WithMany()
                .HasForeignKey(oi => oi.CustomPrintRequestId)
                .IsRequired(false);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .IsRequired(false);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.PrintingService)
                .WithMany(ps => ps.Reviews)
                .HasForeignKey(r => r.PrintingServiceId)
                .IsRequired(false);
        }
    }
}
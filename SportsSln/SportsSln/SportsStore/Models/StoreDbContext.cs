using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartLine> CartLines { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure CartLine relationships
            modelBuilder.Entity<CartLine>()
                .HasOne(cl => cl.Product)
                .WithMany()
                .HasForeignKey(cl => cl.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartLine>()
                .HasOne<Order>()
                .WithMany(o => o.Lines)
                .HasForeignKey(cl => cl.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}

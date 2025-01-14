using Microsoft.EntityFrameworkCore;
using ThAmCo.Main.Models; 

namespace ThAmCo.Main.Data
{
    public class CoreServiceContext : DbContext
    {
        public CoreServiceContext(DbContextOptions<CoreServiceContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }

}

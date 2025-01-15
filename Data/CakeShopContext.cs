using Microsoft.EntityFrameworkCore;
using ShopCake.Models;

namespace ShopCake.Data
{
    public class CakeShopContext : DbContext
    {
        public CakeShopContext(DbContextOptions<CakeShopContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        // Thêm các DbSet khác nếu cần

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.ORD_ID);
                
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.USE_ID);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.MEM_ID)
                    .IsRequired(false);
            });

           
        }
    }
} 
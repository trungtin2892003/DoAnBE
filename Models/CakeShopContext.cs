using Microsoft.EntityFrameworkCore;

namespace ShopCake.Models
{
    public class CakeShopContext:DbContext
    {
        public CakeShopContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Team> Teams { get; set; } // Thêm bảng Team
        public DbSet<Cart> Carts { get; set; } // Thêm bảng Carts
        public DbSet<CartDetail> CartDetails { get; set; } // Thêm bảng CartDetails
        public DbSet<ContactForm> ContactForms { get; set; }

    }
}

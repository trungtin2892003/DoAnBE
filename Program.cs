using ShopCake.Models;
using Microsoft.EntityFrameworkCore;
using ShopCake.Controllers;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Areas.Admin.Service;

namespace ShopCake
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();  // Xóa các nhà cung cấp logging mặc định nếu cần
            builder.Logging.AddConsole();      // Thêm log vào console (mặc định)
            builder.Logging.AddDebug();
            // Thêm dịch vụ vào container DI
            builder.Services.AddScoped<CartService>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<EmailService>();


            // Đăng ký DbContext
            builder.Services.AddDbContext<CakeShopContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Cấu hình Cache và Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1); // Thời gian hết hạn của session
                options.Cookie.HttpOnly = true;             // Chỉ cho phép truy cập cookie qua HTTP
                options.Cookie.IsEssential = true;          // Đảm bảo tương thích với GDPR
            });

            // Thêm dịch vụ MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Cấu hình pipeline HTTP request
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
          

            // Kích hoạt session (bắt buộc để sử dụng session)
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Định nghĩa route cho khu vực (areas)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // Định nghĩa route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

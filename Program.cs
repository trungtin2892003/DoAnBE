﻿using ShopCake.Models;
using Microsoft.EntityFrameworkCore;

namespace ShopCake
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

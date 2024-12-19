using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly CakeShopContext _context;

        public UserController(CakeShopContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }
        // Action để xử lý Sign Out
        public IActionResult Logout()
        {
            // Xóa thông tin session của người dùng
            HttpContext.Session.Clear();

            // Chuyển hướng về trang Login
            return RedirectToAction("Login", "User");
        }
        public IActionResult Login()
        {
            // Đọc LoginDTO từ Cookie
            var cookie = Request.Cookies["UserCredential"];
            if (!string.IsNullOrEmpty(cookie))
            {
                var login = JsonSerializer.Deserialize<LoginDTO>(cookie);
                var result = _context.AdminUsers.AsNoTracking()
                    .FirstOrDefault(x => x.UserName.ToString() == login.Username &&
                                         x.Password.ToString() == login.Password);


                if (result != null)
                {
                    HttpContext.Session.Set("userInfo", result);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginDTO login)
        {
            var result = await _context.AdminUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName.ToString() == login.Username && x.Password.ToString() == login.Password);
            Console.WriteLine(result?.UserName);
            if (result != null)
            {
                // Serialize AdminUser và lưu vào Session
                HttpContext.Session.Set("userInfo", result);

                // Serialize LoginDTO và lưu vào Cookie
                var cookieValue = JsonSerializer.Serialize(login);
                Response.Cookies.Append("UserCredential", cookieValue, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true, // Tăng cường bảo mật
                    Secure = true
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Message"] = "Wrong username or password";
            }
            return View();
        }

    }
}

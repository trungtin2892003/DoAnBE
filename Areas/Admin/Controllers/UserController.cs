using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ShopCake.Unity;

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

        // GET: Login page
        [HttpGet]
        public IActionResult Login()
        {
            // Kiểm tra cookie để tự động đăng nhập
            var cookie = Request.Cookies["UserCredential"];
            if (!string.IsNullOrEmpty(cookie))
            {
                var login = JsonSerializer.Deserialize<LoginDTO>(cookie);
                var result = _context.AdminUsers.AsNoTracking()
                    .FirstOrDefault(x => x.UserName == login.Username &&
                                         x.Password == login.Password); // Chưa hash

                if (result != null)
                {
                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.Set("userInfo", result);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            // Kiểm tra thông tin đăng nhập trong database
            var result = await _context.AdminUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == login.Username &&
                                          x.Password == login.Password); // Chưa hash

            if (result != null)
            {
                // Lưu thông tin người dùng vào session
                HttpContext.Session.Set("userInfo", result);

                // Lưu thông tin đăng nhập vào cookie
                var cookieValue = JsonSerializer.Serialize(login);
                Response.Cookies.Append("UserCredential", cookieValue, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true // Chỉ hoạt động trên HTTPS
                });

                // Chuyển hướng về trang Index
                return RedirectToAction("Index", "Home");
            }

            // Nếu sai thông tin, hiển thị thông báo
            ViewData["Message"] = "Wrong username or password";
            return View();
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Xóa thông tin session của người dùng
            HttpContext.Session.Clear();

            // Xóa cookie đăng nhập
            Response.Cookies.Delete("UserCredential");

            // Chuyển hướng về trang Login
            return RedirectToAction("Login","User");
        }

        // GET: Index
        public IActionResult Index()
        {
            return View();
        }
    }
}

using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ShopCake.Unity;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;
using ShopCake.Areas.Admin.Service;


namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly EmailService _emailService;

        public UserController(CakeShopContext context, ILogger<UserController> logger, EmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;

        }

        // GET: Login page
        [HttpGet]
        public IActionResult Login()
        {

            var cookie = Request.Cookies["UserCredential"];
            if (!string.IsNullOrEmpty(cookie))
            {
                var login = JsonSerializer.Deserialize<LoginDTO>(cookie);
                var user = _context.AdminUsers.AsNoTracking()
                    .FirstOrDefault(x => x.UserName == login.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                {
                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.Set("userInfo", user);
                    HttpContext.Session.SetInt32("USE_ID", user.USE_ID);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (user.Role == "")
                    {
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please fill in all fields correctly.";
                return View();
            }

            // Kiểm tra username/email đã tồn tại
            var existingUser = await _context.AdminUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == register.UserName || x.Email == register.Email);

            if (existingUser != null)
            {
                ViewData["Message"] = "Username or Email already exists.";
                return View();
            }

            // Tạo user mới
            var newUser = new AdminUser
            {
                UserName = register.UserName,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                DisplayName = register.UserName,
                CreatedDate = DateTime.UtcNow
            };

            _context.AdminUsers.Add(newUser);
            await _context.SaveChangesAsync();

            // Gửi email xác nhận
            await _emailService.SendEmailAsync(newUser.Email, "Registration Successful",
                $"Dear {newUser.DisplayName},\n\nYour registration was successful. Welcome to our platform!");

            return RedirectToAction("Login", "User", new { Area = "Admin" });
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var user = await _context.AdminUsers
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.UserName == login.Username);

            if (user != null)
            {
                // Kiểm tra mật khẩu đã mã hóa
                if (BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                {
                    // Kiểm tra role có phải null không
                    if (user.Role == null)
                    {
                        ModelState.AddModelError(string.Empty, "Your account does not have a valid role.");
                        return View();
                    }

                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.Set("userInfo", user);
                    if (user.Role == "Admin")
                    {
                        HttpContext.Session.SetInt32("Admin_USE_ID", user.USE_ID);
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("User_USE_ID", user.USE_ID);
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(); ;
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
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");

            // Kiểm tra xem người dùng có phải admin không
            if (userInfo == null || userInfo.Role != "Admin")
            {
                // Nếu không phải admin, chuyển hướng về trang chính hoặc hiển thị thông báo lỗi
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

    }
}

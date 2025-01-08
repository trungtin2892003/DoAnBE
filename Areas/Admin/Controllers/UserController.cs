using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ShopCake.Unity;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;


namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly ILogger<UserController> _logger;
        public UserController(CakeShopContext context, ILogger<UserController> logger)
        {
            _logger = logger;
            _context = context;
            
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
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password), // Mã hóa password
                DisplayName = register.UserName, // Hoặc gán giá trị phù hợp
                CreatedDate = DateTime.UtcNow
            };


            _context.AdminUsers.Add(newUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login", "User", new { Area = "Admin" });

        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            // Tìm kiếm người dùng theo tên đăng nhập
            var user = await _context.AdminUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == login.Username);

            if (user != null)
            {
                // Kiểm tra mật khẩu đã mã hóa
                if (BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                {
                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.Set("userInfo", user);
                    if (user.Role == "Admin")
                    {
                        // Lưu session dành riêng cho admin
                        HttpContext.Session.SetInt32("Admin_USE_ID", user.USE_ID);

                        
                    }
                    else if (user.Role == "")
                    {
                        // Lưu session dành riêng cho user
                        HttpContext.Session.SetInt32("User_USE_ID", user.USE_ID);

                        
                    }

                    // Lưu thông tin đăng nhập vào cookie
                    var cookieValue = JsonSerializer.Serialize(new LoginDTO
                    {
                        Username = login.Username,
                        Password = user.Password // Lưu mật khẩu mã hóa vào cookie
                    });
                    Response.Cookies.Append("UserCredential", cookieValue, new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = true,
                        Secure = true // Chỉ hoạt động trên HTTPS
                    });

                    // Phân quyền theo Role
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }
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
            // Lấy tên người dùng từ session
            var userName = HttpContext.Session.GetString("UserName");

            // Chuyển dữ liệu vào View
            ViewBag.UserName = userName;

            return View();
        }

    }
}

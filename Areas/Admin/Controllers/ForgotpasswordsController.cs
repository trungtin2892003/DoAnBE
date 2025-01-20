using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ForgotpasswordsController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<UserController> _logger;


        public ForgotpasswordsController(CakeShopContext context, ILogger<UserController> logger, EmailService emailService)

        {
           
            _context = context;
            _emailService = emailService;
        }


            [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please enter a valid email.";
                return View();
            }

            // Kiểm tra email có tồn tại không
            var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == forgotPassword.Email);
            if (user == null)
            {
                ViewData["Message"] = "Email không tồn tại trong hệ thống.";
                return View();
            }

            // Tạo token đặt lại mật khẩu
            var token = Guid.NewGuid().ToString();

            // Lưu token và hạn sử dụng vào database
            user.ResetPasswordToken = token;
            user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1); // Token có hiệu lực 1 giờ
            _context.AdminUsers.Update(user);
            await _context.SaveChangesAsync();

            // Tạo liên kết đặt lại mật khẩu
            var resetLink = Url.Action("ResetPassword", "User", new { token = token }, protocol: Request.Scheme);

            // Gửi email đặt lại mật khẩu
            await _emailService.SendEmailAsync(
                forgotPassword.Email,
                "Password Reset Request",
                $"Click vào liên kết sau để đặt lại mật khẩu của bạn: <a href='{resetLink}'>{resetLink}</a>"
            );

            ViewData["Message"] = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn.";
            return View();
        }

    }
}

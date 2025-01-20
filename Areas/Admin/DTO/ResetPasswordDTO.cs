using System.ComponentModel.DataAnnotations;

namespace ShopCake.Areas.Admin.DTO
{
    public class ResetPasswordDTO
    {
        public string? Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự.")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string? ConfirmPassword
        {
            get; set;
        }
    }
}

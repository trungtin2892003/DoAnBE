using System.ComponentModel.DataAnnotations;

namespace ShopCake.Areas.Admin.DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Users")]
    public class AdminUser : BaseModel
    {
        [Key]
        public int USE_ID { get; set; }
        [Required]
        [MaxLength(255)]
        public required string UserName { get; set; }
        //[Required]
        //public required string DisplayName { get; set; }
        [MaxLength(255)]
        public required string Password { get; set; }



        public string? Email { get; set; }

    }
}
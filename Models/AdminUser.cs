using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace ShopCake.Models
{
    [Table("Users")]
    public class AdminUser 
    {
        [Key]
        public int USE_ID { get; set; }
        [Required]
        [MaxLength(255)]
        public required string UserName { get; set; }
<<<<<<< HEAD
=======

>>>>>>> dc9e599519289a7c5ee66623e071d2e7fa9edef8
        [Required]
        public required string DisplayName { get; set; }


        public required string Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
       
        public DateTime updatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
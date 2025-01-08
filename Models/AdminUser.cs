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
        public string DisplayName { get; set; }
        //[Required]
        //public required string DisplayName { get; set; }
=======
        [Required]
        public required string DisplayName { get; set; }
>>>>>>> 9b1d73445bfdc32755d9986e620eb2208a011d18
        [MaxLength(255)]
        public required string Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
       
        public DateTime updatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
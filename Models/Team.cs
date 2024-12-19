using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCake.Models
{
    [Table("Team")]
    public class Team:BaseModel
    {
        [Key]
        public int TEAM_ID { get; set; } // Mã định danh thành viên

        [Required]
        [StringLength(255)]
        public string? Name { get; set; } // Tên thành viên

        [StringLength(255)]
        public string? Position { get; set; } // Chức vụ của thành viên

        [StringLength(500)]
        public string? PhotoUrl { get; set; } // URL ảnh đại diện

        [StringLength(500)]
        public string? FacebookUrl { get; set; } // Link Facebook

        [StringLength(500)]
        public string? InstagramUrl { get; set; } // Link Instagram


    }
}

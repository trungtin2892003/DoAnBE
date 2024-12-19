using System.ComponentModel.DataAnnotations;

namespace ShopCake.Areas.Admin.DTO
{
    public class TeamDTO
    {
        public int TEAM_ID { get; set; } // Mã định danh thành viên

  
        public string? Name { get; set; } // Tên thành viên

        public string? Position { get; set; } // Chức vụ của thành viên

        
        public IFormFile? PhotoUrl { get; set; } // URL ảnh đại diện

        
        public string? FacebookUrl { get; set; } // Link Facebook

       
        public string? InstagramUrl { get; set; } // Link Instagram

    }
}

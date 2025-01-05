using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace ShopCake.Areas.Admin.DTO
{
    public class BannerDTO
    {
        public int BAN_ID { get; set; }

        public string? Title { get; set; }

        public IFormFile Image { get; set; }

        public string? Url { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giá trị phải lớn hơn hoặc bằng 1.")]
        public int DisplayOrder { get; set; }

        
    }
}

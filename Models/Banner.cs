using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Banners")]
    public class Banner : BaseModel
    {
        [Key]
        public int BAN_ID { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Url { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá TRỊ phải lớn hơn hoặc bằng 0.")]
        public int DisplayOrder { get; set; }

    }
}

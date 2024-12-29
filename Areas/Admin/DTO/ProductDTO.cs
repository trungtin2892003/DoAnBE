using ShopCake.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCake.Areas.Admin.DTO
{
    public class ProductDTO : BaseModel
    {
        
        public int PRO_ID { get; set; }
        public int CAT_ID { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
       
        public required string Name { get; set; }
        [Range(1, 500, ErrorMessage = "Số lượng sản phẩm phải từ 0 đến 100.")]
        public required int Quantity {  get; set; }
        public IFormFile? Avatar { get; set; }
        public string? Intro { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống.")]
        [Range(0, 1000, ErrorMessage = "Giá sản phẩm phải từ 0 đến 1000.")]
        public required decimal Price { get; set; }

        [Required(ErrorMessage = "Giá giảm không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá giảm không được nhỏ hơn 0.")]
        public required decimal DiscountPrice { get; set; }

        public string? Unit { get; set; }

        [Range(0, 5, ErrorMessage = "Đánh giá phải nằm trong khoảng từ 0 đến 5.")]
        public double? Rate { get; set; }

        public string? Description { get; set; }
        public string? Details { get; set; }
    }
}

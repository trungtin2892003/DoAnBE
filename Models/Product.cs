using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopCake.Models
{
    [Table("Products")]
    public class Product:BaseModel
    {
        [Key]
        public int PRO_ID { get; set; }
        public int CAT_ID { get; set; }
        [StringLength(125, ErrorMessage = "Name cannot exceed 125 characters.")]
        public string? Name { get; set; }
        [DisplayName("Số lượng")]
        [Range(1, 500, ErrorMessage = "Số lượng sản phẩm phải từ 0 đến 100.")]
        public int? Quantity { get; set; }
        public string? Avatar { get; set; }
        public string? Intro { get; set; }
        [Range(1, 1000, ErrorMessage = "Giá sản phẩm phải từ 0 đến 1000.")]
        [DisplayFormat(DataFormatString = "{0:0,0.000} VND")]
        public decimal Price { get; set; }
        [Range(1, 1000, ErrorMessage = "Giảm giá sản phẩm phải từ 0 đến 1000.")]
        [DisplayFormat(DataFormatString = "{0:0,0.000} VND")]
        public decimal DiscountPrice { get; set; }
        public string? Unit { get; set; }
        [DisplayName("Đánh giá")]
        public double? Rate { get; set; }
        public string? Description { get; set; }
        public string? Details { get; set; }
        public int? SlOrder {  get; set; }

        [ForeignKey("CAT_ID")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetail { get; set; }

        public virtual ICollection<Review>? Review { get; set; }
    }
}

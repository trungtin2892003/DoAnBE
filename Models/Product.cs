using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Products")]
    public class Product:BaseModel
    {
        [Key]
        public int PRO_ID { get; set; }
        public int CAT_ID { get; set; }
        [Required]
        public string? Avatar { get; set; }
       
        public string? Name { get; set; }
        
        public string? Intro { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "giảm giá sản phẩm phải lớn hơn hoặc bằng 0.")]
        public decimal DiscountPrice { get; set; }
        public string? Unit { get; set; }
        public double? Rate { get; set; }
        public string? Description { get; set; }
        public string? Details { get; set; }

        [ForeignKey("CAT_ID")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetail { get; set; }

        public virtual ICollection<Review>? Review { get; set; }
    }
}

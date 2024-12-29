using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCake.Models
{
    [Table("CartDetails")]
    public class CartDetail
    {
        [Key]
        public int CARD_ID { get; set; } // Mã chi tiết giỏ hàng
        public string? ProductName { get; set; }
        public string?   ProductImage { get; set; }
        [Required]
        public int CAR_ID { get; set; } // Mã giỏ hàng

        [Required]
        public int PRO_ID { get; set; } // Mã sản phẩm

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; } // Số lượng sản phẩm

        [Required]
        public decimal Price { get; set; } // Giá sản phẩm
        public decimal Total
        {
            get => Quantity * Price;
            set { } // Thêm setter rỗng (nếu cần gán)
        }
        public decimal? DiscountPrice { get; set; } // Giá sau giảm giá (nullable)

        [ForeignKey("CAR_ID")]
        public virtual Cart? Cart { get; set; } // Quan hệ với giỏ hàng

        [ForeignKey("PRO_ID")]
        public virtual Product? Product { get; set; } // Quan hệ với sản phẩm
    }
}

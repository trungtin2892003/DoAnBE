using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCake.Models
{
    [Table("Carts")]
    public class Cart 
    {
        [Key]
        public int CAR_ID { get; set; } // Mã giỏ hàng
        public int PRO_ID { get; set; } // Mã sản phẩm
        public string? ProductName { get; set; }

        public string? ProductImage { get; set; }
        public string? SessionId { get; set; }
        public int USE_ID { get; set; }
        public int Quantity { get; set; }
       
        public DateTime CartDate { get; set; } = DateTime.Now;
        public string? CustomerName { get; set; }
        public decimal Price { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal TotalPrice
        {
            get => Quantity * Price;
            set { } // Thêm setter rỗng (nếu cần gán)
        }
        public double? Discount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }
 
        // Quan hệ với các bảng khác
        [ForeignKey("USE_ID")]
        public virtual Order? User { get; set; } // Quan hệ với bảng Users



        public virtual ICollection<CartDetail>? CartDetails { get; set; } // Quan hệ với chi tiết giỏ hàng
    }
}

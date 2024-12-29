using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCake.Models
{
    [Table("Carts")]
    public class Cart : BaseModel
    {
        [Key]
        public int CAR_ID { get; set; } // Mã giỏ hàng

        public int USE_ID { get; set; }
        public int MEM_ID { get; set; }
        public DateTime CartDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }
 
        // Quan hệ với các bảng khác
        [ForeignKey("USE_ID")]
        public virtual Order? User { get; set; } // Quan hệ với bảng Users

        [ForeignKey("MEM_ID")]
        public virtual Member? Member { get; set; } // Quan hệ với bảng Members

        public virtual ICollection<CartDetail>? CartDetails { get; set; } // Quan hệ với chi tiết giỏ hàng
    }
}

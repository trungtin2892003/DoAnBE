using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Orders")]
    public class Order : BaseModel
    {
        [Key]
        public int ORD_ID { get; set; }
        public int USE_ID { get; set; }
        public required int MEM_ID { get; set; }
        public required DateTime OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int TotalPrice { get; set; } // Nếu SQL giữ kiểu int
        public double Discount { get; set; } // Phù hợp với kiểu float trong SQL
        public string? PaymentMethod { get; set; }
        public bool IsPaid { get; set; } // Đúng với kiểu bit trong SQL
        public string? Note { get; set; }
        public int Status { get; set; }

        [ForeignKey("MEM_ID")]
        public virtual Member? Member { get; set; }
        [ForeignKey("USE_ID")]
        public virtual AdminUser? User { get; set; }
    }
}

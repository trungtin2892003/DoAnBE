using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCake.Models
{
    [Table("Carts")]
    public class Cart:BaseModel
    {
        [Key]
        public int CAR_ID { get; set; } // Mã giỏ hàng

        [Required]
        public int USE_ID { get; set; } // Mã người dùng (User)

        [Required]
        public int MEM_ID { get; set; } // Mã thành viên (Member)

        public DateTime CartDate { get; set; } = DateTime.Now; // Ngày tạo giỏ hàng

        [StringLength(255)]
        public string? CustomerName { get; set; } // Tên khách hàng

        [StringLength(20)]
        public string? Phone { get; set; } // Số điện thoại

        [StringLength(255)]
        public string? Address { get; set; } // Địa chỉ giao hàng

        public decimal TotalPrice { get; set; } // Tổng giá trị đơn hàng

        public float Discount { get; set; } // Giảm giá (nếu có)

        [StringLength(50)]
        public string? PaymentMethod { get; set; } // Phương thức thanh toán

        [StringLength(500)]
        public string? Note { get; set; } // Ghi chú của khách hàng

        [Required]
        public int Status { get; set; } // Trạng thái đơn hàng (ví dụ: 0 - đang xử lý, 1 - hoàn thành)


        // Quan hệ với các bảng khác
        [ForeignKey("USE_ID")]
        public virtual Order? User { get; set; } // Quan hệ với bảng Users

        [ForeignKey("MEM_ID")]
        public virtual Member? Member { get; set; } // Quan hệ với bảng Members

        public virtual ICollection<CartDetail>? CartDetails { get; set; } // Quan hệ với chi tiết giỏ hàng
    }
}

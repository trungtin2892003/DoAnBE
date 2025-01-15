namespace ShopCake.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OrderCreateModel:BaseModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; }

    }
} 
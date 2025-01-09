using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCake.Models
{
    [Table("Carts")]
    public class Cart 
    {
        [Key]
        public int CAR_ID { get; set; }
        public int PRO_ID { get; set; }
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
            set { }
        }
        public decimal? Discount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }

        [ForeignKey("USE_ID")]
        public virtual AdminUser? User { get; set; }

        public virtual ICollection<CartDetail>? CartDetails { get; set; }
    }
}

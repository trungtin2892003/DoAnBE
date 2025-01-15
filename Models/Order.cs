﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Orders")]
    public class Order : BaseModel
    {
        [Key]
        public int ORD_ID { get; set; }
        
        public int? USE_ID { get; set; }
        public int? MEM_ID { get; set; }
        public required DateTime OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal TotalPrice { get; set; }
        public double Discount { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }

        [ForeignKey("USE_ID")]
        public virtual AdminUser? User { get; set; }

        [ForeignKey("MEM_ID")]
        public virtual Member? Member { get; set; }

        //public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Reviews")]
    public class Review 
    {
        [Key]
        public int REV_ID { get; set; }
        public required int MEM_ID { get; set; }
        public string Productname { get; set; }
        public required int PRO_ID { get; set; }
        public required double Rate { get; set; }
        public required string Content { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        [ForeignKey("MEM_ID")]
        public virtual Member? Member { get; set; }

        [ForeignKey("PRO_ID")]
        public virtual Product? Product { get; set; }
    }
}

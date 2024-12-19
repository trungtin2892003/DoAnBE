using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopCake.Models
{
    [Table("Categories")]
    public class Category : BaseModel
    {
        [Key]
        public int CAT_ID { get; set; }
        public required string Name { get; set; }
        [DisplayName("DisplayOrder")]
        public int DisplayOrder { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}
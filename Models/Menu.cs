using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Menus")]
    public class Menu : BaseModel
    {
        [Key]
        public int MEN_ID { get; set; }
        public int PARENT_ID { get; set; }
        public required string Title { get; set; }
        public string? Url { get; set; }

        [ForeignKey("PARENT_ID")]
        public virtual ICollection<Menu>? children { get; set; }
    }
}

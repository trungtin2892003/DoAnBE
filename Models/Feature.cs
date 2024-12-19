using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopCake.Models
{
    [Table("Features")]
    public class Feature : BaseModel
    {
        [Key]
        public int FEA_ID { get; set; }
        public required string Icon { get; set; }
        public required string Title { get; set; }
        public string? Subtitle { get; set; }
        public int DisplayOrder { get; set; }

    }
}

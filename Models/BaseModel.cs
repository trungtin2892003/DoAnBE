using System.ComponentModel;

namespace ShopCake.Models
{
    public class BaseModel
    {
        [DisplayName("Created date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [DisplayName("Create By")]
        public string? createdBy { get; set; }

        [DisplayName("Updated date")]
        public DateTime updatedDate { get; set; } = DateTime.Now;
        [DisplayName("Updated By")]
        public string? updatedBy { get; set; }
    }
}

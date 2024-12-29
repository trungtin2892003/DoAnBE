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
        [StringLength(20, ErrorMessage = "Tên danh mục không được vượt quá 20 ký tự.")]
        public  string Name { get; set; }
        [DisplayName("Thứ tự sắp xếp")]
        [Range(1, double.MaxValue, ErrorMessage = "GIÁ TRỊ phải lớn hơn0.")]
        public int DisplayOrder { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}
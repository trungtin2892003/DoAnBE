using System.ComponentModel.DataAnnotations;
using ShopCake.Models;

namespace ShopCake.Areas.Admin.DTO
{
    public class CategoryDTO :BaseModel
    {
        public string name { get; set;}

        [Range(1, int.MaxValue, ErrorMessage = "Giá trị phải lớn hơn hoặc bằng 1.")]

        public int DisplayOrder { get; set; }

    }
}

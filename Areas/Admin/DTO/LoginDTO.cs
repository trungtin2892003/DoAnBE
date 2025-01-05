
namespace ShopCake.Areas.Admin.DTO
{
    public class LoginDTO
    {
        public int USE_ID {  get; set; }
        public string? Username {  get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        public string? Role {  get; set; }
    }
}

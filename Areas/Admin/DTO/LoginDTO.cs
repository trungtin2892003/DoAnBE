namespace ShopCake.Areas.Admin.DTO
{
    public class LoginDTO
    {
        public int userId {  get; set; }
        public string? Username {  get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

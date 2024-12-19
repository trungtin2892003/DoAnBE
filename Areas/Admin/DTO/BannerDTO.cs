namespace ShopCake.Areas.Admin.DTO
{
    public class BannerDTO
    {
        public int BAN_ID { get; set; }
        public string? Title { get; set; }
        public IFormFile? Image { get; set; }
        public string? Url { get; set; }
        public int DisplayOrder { get; set; }

    }
}

namespace ShopCake.Areas.Admin.DTO
{
    public class MemberDTOcs
    {
        public required string LastName { get; set; }

        public required string FirstName { get; set; }
        public string? Phone { get; set; }
        public required string Email { get; set; }
        public string? Address { get; set; }

    }
}

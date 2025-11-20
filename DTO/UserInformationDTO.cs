namespace WebApplication1.DTO
{
    public class UserInformationDTO
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public int RegionId { get; set; }
        public IFormFile Avatar { get; set; }
    }
}

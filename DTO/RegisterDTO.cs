namespace WebApplication1.DTO
{
    public class RegisterDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }     
        public int RegionId { get; set; }
        public int OTP { get; set; }
    }
}

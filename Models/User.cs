namespace WebApplication1.Models;
public class User 
{
    public int userId { get; set; }
    public required string username { get; set; }
    public Region? region { get; set; }
    
    public string? linkAvatar { get; set; }
    public Role? role { get; set; }
    
    public int otp { get; set; }
}
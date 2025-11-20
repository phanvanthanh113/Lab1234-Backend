namespace WebApplication1.Models;

public class User 
{
    public int userId { get; set; }

    public required string username { get; set; }

    public int regionId { get; set; }
    public int roleId { get; set; }

    public string? linkAvatar { get; set; }

    public int otp { get; set; }
    public Region? region { get; set; }
    public Role? role { get; set; }
}

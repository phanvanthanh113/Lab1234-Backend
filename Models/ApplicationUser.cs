using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public int? RegionId { get; set; }
    public string? Name { get; set; }

    public bool IsDeleted { get; set; } = false;

    public string OTP { get; set; }
        = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + "none";
}

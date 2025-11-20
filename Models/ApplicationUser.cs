using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models;

public class ApplicationUser : IdentityUser
{
    // thêm nếu muốn, không bắt buộc
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
}

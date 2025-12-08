using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // ✅ CHỈ LẤY USER CHƯA BỊ XÓA (IsDeleted = false)
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Where(u => u.IsDeleted == false)
                .Select(u => new
                {
                    username = u.UserName,
                    email = u.Email,
                    regionId = u.RegionId,
                    role = new
                    {
                        name = "User"
                    },
                    region = new
                    {
                        name = "Region " + u.RegionId
                    }
                })
                .ToListAsync();

            return Ok(users);
        }

        // ✅ LẤY 1 USER THEO ID (CHƯA BỊ XÓA)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == id && u.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            var data = new
            {
                username = user.UserName,
                email = user.Email,
                regionId = user.RegionId,
                role = new { name = "User" },
                region = new { name = "Region " + user.RegionId }
            };

            return Ok(data);
        }
    }
}

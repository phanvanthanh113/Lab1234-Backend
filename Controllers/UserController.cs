namespace WebApplication1.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
[Route("api/[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users
            .Include(u => u.role)
            .Include(u => u.region)
            .ToListAsync();
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.Include(u => u.role).Include(u => u.region).FirstOrDefaultAsync(u => u.userId == id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.userId}, user);
    }

}
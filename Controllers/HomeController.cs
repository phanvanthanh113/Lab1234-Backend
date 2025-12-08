using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModel;
using WebApplication1.Data;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ================= HOME =================
        public IActionResult Index()
        {
            var users = _userManager.Users
                .Where(u => !u.IsDeleted)
                .ToList();

            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ================= REGISTER GET =================
        public IActionResult Register()
        {
            var regions = _context.Regions.ToList();

            RegisterVM vm = new RegisterVM
            {
                Email = "",
                OTP = "",
                Name = "",
                LinkAvatar = "",
                RegionId = 1,
                Regions = new SelectList(regions, "regionId", "Name")
            };

            return View(vm);
        }

        // ================= REGISTER POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]   // ✅ BỔ SUNG CHỐNG CSRF (ĐÚNG CHUẨN MVC)
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            ModelState.Remove("Regions");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registerVM.Name,     // ✅ GIỮ ĐÚNG YÊU CẦU: USERNAME = NAME
                    Email = registerVM.Email,
                    Name = registerVM.Name,
                    RegionId = registerVM.RegionId,
                    OTP = registerVM.OTP
                };

                var result = await _userManager.CreateAsync(user, registerVM.OTP); // ✅ OTP là password

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");  // ✅ GÁN ROLE ADMIN
                    await _signInManager.SignInAsync(user, isPersistent: false); // ✅ AUTO LOGIN
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // ✅ NẠP LẠI REGION KHI LỖI
            var regions = await _context.Regions.ToListAsync();
            registerVM.Regions = new SelectList(regions, "regionId", "Name");

            return View(registerVM);
        }

        // ================= LOGOUT =================
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ================= ACCESS DENIED =================
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
        // ================= LOGIN GET =================
public IActionResult Login()
{
    return View();
}

// ================= LOGIN POST (USERNAME + OTP) =================
[HttpPost]
public async Task<IActionResult> Login(LoginVM loginVM)
{
    if (ModelState.IsValid)
    {
        var result = await _signInManager.PasswordSignInAsync(
            loginVM.UserName,   // UserName
            loginVM.OTP,        // OTP = Password
            true,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(loginVM.UserName);
            var roles = await _userManager.GetRolesAsync(user!);

            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        ModelState.AddModelError(string.Empty, "Sai Username hoặc OTP");
    }

    return View(loginVM);
}

    }
}

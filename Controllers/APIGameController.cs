using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;
using WebApplication1.DTO;


namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIGameController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected ResponseAPI _response;
        private readonly UserManager<ApplicationUser> _userManager;

        public APIGameController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _response = new ResponseAPI();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (model == null)
            {
                _response.status = 400;
                _response.message = "Dữ liệu không hợp lệ";
                return BadRequest(_response);
            }

            // Tạo user mới
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName
            };

            // Tạo user bằng Identity
            var result = await _userManager.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                _response.status = 400;
                _response.message = "Đăng ký thất bại";
                _response.data = result.Errors;
                return BadRequest(_response);
            }

            _response.status = 200;
            _response.message = "Đăng ký thành công";
            _response.data = user;

            return Ok(_response);
        }

        [HttpGet("GetAllGameLevel")]
        public IActionResult GetAllGameLevel()
        {
            var data = _db.GameLevels.ToList();

            _response.status = 200;
            _response.message = "Lấy danh sách Level thành công";
            _response.data = data;

            return Ok(_response);
        }
        [HttpGet("GetAllQuestionGame")]
        public IActionResult GetAllQuestionGame()
        {
            var data = _db.Questions.ToList();

            _response.status = 200;
            _response.message = "Lấy danh sách câu hỏi thành công";
            _response.data = data;

            return Ok(_response);
        }
        [HttpGet("GetAllRegion")]
        public IActionResult GetAllRegion()
        {
            var data = _db.Regions.ToList();

            _response.status = 200;
            _response.message = "Lấy danh sách Region thành công";
            _response.data = data;

            return Ok(_response);
        }
    }
}

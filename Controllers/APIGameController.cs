using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;
using WebApplication1.DTO;
using Microsoft.EntityFrameworkCore;
using WebApplication1.ViewModel;
using WebApplication1.Services;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIGameController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected ResponseAPI _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public APIGameController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _response = new ResponseAPI();
        }
        private string GenerateJwtToken(string userId, string userName)
{
    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

    var credentials = new SigningCredentials(
        key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, userName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, userId)
    };

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

        // ================== BÀI 3 – REGISTER ==================
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (model == null)
            {
                _response.IsSuccess = false;
                _response.Notification = "Dữ liệu không hợp lệ";
                _response.Data = null;
                return BadRequest(_response);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                RegionId = model.RegionId,
                OTP = model.OTP.ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                _response.IsSuccess = false;
                _response.Notification = "Đăng ký thất bại";
                _response.Data = result.Errors;
                return BadRequest(_response);
            }

            _response.IsSuccess = true;
            _response.Notification = "Đăng ký thành công";
            _response.Data = user;
            return Ok(_response);
        }

        // ================== BÀI 3 – LOGIN ==================
[HttpPost("Login")]
public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
{
    try
    {
        var username = loginRequest.UserName!;
        var otp = loginRequest.OTP.ToString();

        // ✅ LOGIN BẰNG IDENTITY USER (AspNetUsers)
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            _response.IsSuccess = false;
            _response.Notification = "Sai username";
            return BadRequest(_response);
        }

        if (user.OTP != otp)
        {
            _response.IsSuccess = false;
            _response.Notification = "Sai OTP";
            return BadRequest(_response);
        }

        // ✅ TẠO JWT TOKEN
        var token = GenerateJwtToken(user.Id, user.UserName!);

        var data = new
        {
            token = token,
            user = user
        };

        _response.IsSuccess = true;
        _response.Notification = "Đăng nhập thành công";
        _response.Data = data;

        return Ok(_response);
    }
    catch (Exception ex)
    {
        _response.IsSuccess = false;
        _response.Notification = "Lỗi";
        _response.Data = ex.Message;
        return BadRequest(_response);
    }
}


        // ================== GET ALL GAME LEVEL ==================
        [HttpGet("GetAllGameLevel")]
        public IActionResult GetAllGameLevel()
        {
            var data = _db.GameLevels.ToList();

            _response.IsSuccess = true;
            _response.Notification = "Lấy danh sách Level thành công";
            _response.Data = data;

            return Ok(_response);
        }

        // ================== GET ALL QUESTION ==================
        [HttpGet("GetAllQuestionGame")]
        public IActionResult GetAllQuestionGame()
        {
            var data = _db.Questions.ToList();

            _response.IsSuccess = true;
            _response.Notification = "Lấy danh sách câu hỏi thành công";
            _response.Data = data;

            return Ok(_response);
        }

        // ================== GET ALL REGION ==================
        [HttpGet("GetAllRegion")]
        public IActionResult GetAllRegion()
        {
            var data = _db.Regions.ToList();

            _response.IsSuccess = true;
            _response.Notification = "Lấy danh sách Region thành công";
            _response.Data = data;

            return Ok(_response);
        }

        // ================== GET QUESTION BY LEVEL ==================
        [HttpGet("GetAllQuestionGameByLevel/{levelId}")]
        public async Task<IActionResult> GetAllQuestionGameByLevel(int levelId)
        {
            try
            {
                var questionGame = await _db.Questions
                                            .Where(x => x.levelId == levelId)
                                            .ToListAsync();

                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = questionGame;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        // ================== SAVE RESULT ==================
        // ================== SAVE RESULT ==================
[HttpPost("SaveResult")]
public async Task<IActionResult> SaveResult([FromBody] LevelResultDTO levelResult)
{
    try
    {
        var levelResultSave = new LevelResult
        {
            userId = levelResult.UserId,   // ✅ GUID
            levelId = levelResult.LevelId,
            score = levelResult.Score,
            completionDate = DateTime.Now
        };

        _db.LevelResults.Add(levelResultSave);
        await _db.SaveChangesAsync();

        _response.IsSuccess = true;
        _response.Notification = "Lưu kết quả thành công";
        _response.Data = levelResultSave;
        return Ok(_response);
    }
    catch (Exception ex)
    {
        _response.IsSuccess = false;
        _response.Notification = "Lỗi";
        _response.Data = ex.Message;
        return BadRequest(_response);
    }
}


        // ================== BÀI 6 – RATING ==================
// [HttpGet("Rating/{idRegion}")]
// public async Task<IActionResult> Rating(int idRegion)
// {
//     try
//     {
//         // Lấy tên khu vực
//         string nameRegion;
//         if (idRegion > 0)
//         {
//             nameRegion = await _db.Regions
//                 .Where(x => x.regionId == idRegion)
//                 .Select(x => x.Name)
//                 .FirstOrDefaultAsync() ?? "Không xác định";
//         }
//         else
//         {
//             nameRegion = "Tất cả";
//         }

//         // 1. Lấy danh sách user từ Identity theo Region
//         IQueryable<ApplicationUser> userQuery = _userManager.Users;

//         if (idRegion > 0)
//         {
//             userQuery = userQuery.Where(u => u.RegionId == idRegion);
//         }

//         var users = await userQuery.ToListAsync();

//         // 2. Lấy tất cả kết quả LevelResult của các user đó (userId là GUID string)
//         var userIds = users.Select(u => u.Id).ToList(); // List<string>

//         var results = await _db.LevelResults
//             .Where(r => userIds.Contains(r.userId))
//             .ToListAsync();

//         // 3. Gộp lại theo từng user
//         RatingVM ratingVM = new RatingVM
//         {
//             NameRegion = nameRegion,
//             userResultSums = new()
//         };

//         foreach (var user in users)
//         {
//             var userResults = results.Where(r => r.userId == user.Id);

//             int sumScore = userResults.Sum(r => r.score);
//             int sumLevel = userResults.Count();

//             UserResultSum userResult = new UserResultSum
//             {
//                 NameUser = user.UserName ?? user.Email ?? "Unknown",
//                 SumScore = sumScore,
//                 SumLevel = sumLevel
//             };

//             ratingVM.userResultSums.Add(userResult);
//         }

//         _response.IsSuccess = true;
//         _response.Notification = "Lấy dữ liệu thành công";
//         _response.Data = ratingVM;
//         return Ok(_response);
//     }
//     catch (Exception ex)
//     {
//         _response.IsSuccess = false;
//         _response.Notification = "Lỗi";
//         _response.Data = ex.Message;
//         return BadRequest(_response);
//     }
// }

        // ================== BÀI 6 – GET USER INFORMATION ==================
        [HttpGet("GetUserInformation/{userId}")]
public async Task<IActionResult> GetUserInformation(string userId)
{
    try
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _response.IsSuccess = false;
            _response.Notification = "Không tìm thấy người dùng";
            _response.Data = null;
            return BadRequest(_response);
        }

        var userInfo = new
        {
            Name = user.FullName,
            Email = user.Email,
            Avatar = user.AvatarUrl,
            Region = "Region " + user.RegionId
        };

        _response.IsSuccess = true;
        _response.Notification = "Lấy dữ liệu thành công";
        _response.Data = userInfo;
        return Ok(_response);
    }
    catch (Exception ex)
    {
        _response.IsSuccess = false;
        _response.Notification = "Lỗi";
        _response.Data = ex.Message;
        return BadRequest(_response);
    }
}
        // ================== BÀI 6 – CHANGE USER PASSWORD ==================
        [HttpPut("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(x => x.Id == changePasswordDTO.UserId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    changePasswordDTO.OldPassword!,
                    changePasswordDTO.NewPassword!
                );

                if (result.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đổi mật khẩu thành công";
                    _response.Data = null;
                    return Ok(_response);
                }

                _response.IsSuccess = false;
                _response.Notification = "Đổi mật khẩu thất bại";
                _response.Data = result.Errors;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        // ================== BÀI 6 – UPDATE USER INFORMATION ==================
        [HttpPut("UpdateUserInformation")]
public async Task<IActionResult> UpdateUserInformation([FromForm] UserInformationDTO userInformationDTO)
{
    try
    {
        // LẤY USER TỪ BẢNG ApplicationUser (Identity)
        var user = await _userManager.FindByIdAsync(userInformationDTO.UserId!);

        if (user == null)
        {
            _response.IsSuccess = false;
            _response.Notification = "Không tìm thấy người dùng";
            _response.Data = null;
            return BadRequest(_response);
        }

        // Cập nhật Tên
        if (!string.IsNullOrEmpty(userInformationDTO.Name))
            user.FullName = userInformationDTO.Name;

        // Cập nhật Region
        if (userInformationDTO.RegionId > 0)
            user.RegionId = userInformationDTO.RegionId;

        // Nếu có file Avatar → upload
        if (userInformationDTO.Avatar != null)
        {
            var ext = Path.GetExtension(userInformationDTO.Avatar.FileName);
            var newFileName = $"{user.Id}{ext}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await userInformationDTO.Avatar.CopyToAsync(stream);
            }

            user.AvatarUrl = newFileName;
        }

        // Lưu thay đổi
        await _userManager.UpdateAsync(user);

        _response.IsSuccess = true;
        _response.Notification = "Cập nhật thông tin thành công";
        _response.Data = user;

        return Ok(_response);
    }
    catch (Exception ex)
    {
        _response.IsSuccess = false;
        _response.Notification = "Lỗi";
        _response.Data = ex.Message;
        return BadRequest(_response);
    }
}


        // ================== BÀI 3 – DELETE ACCOUNT (IsDeleted) ==================
        [HttpDelete("DeleteAccount/{userId}")]
        public async Task<IActionResult> DeleteAccount(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(x => x.Id == userId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                user.IsDeleted = true;
                await _userManager.UpdateAsync(user);

                _response.IsSuccess = true;
                _response.Notification = "Xóa người dùng thành công";
                _response.Data = user;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        // ================== BÀI 5 – FORGOT PASSWORD (GỬI OTP) ==================
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                Random random = new();
                string OTP = random.Next(100000, 999999).ToString();
                user.OTP = OTP;

                await _userManager.UpdateAsync(user);

                string subject = "Reset Password Game 106 - " + Email;
                string message = "Mã OTP của bạn là: " + OTP;

                await _emailService.SendEmailAsync(new EmailRequest
                {
                    ToEmail = Email,
                    Subject = subject,
                    Message = message
                });

                _response.IsSuccess = true;
                _response.Notification = "Gửi mã OTP thành công";
                _response.Data = "OTP sent to " + Email;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        // ================== BÀI 5 – CHECK OTP ==================
        [HttpPost("CheckOTP")]
        public async Task<IActionResult> CheckOTP(CheckOTPDTO checkOTPDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(checkOTPDTO.Email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                // Sửa lỗi so sánh string với int (nếu OTP trong DTO là int)
                var otpInput = checkOTPDTO.OTP.ToString();

                if (user.OTP == otpInput)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Mã OTP chính xác";
                    _response.Data = null;
                    return Ok(_response);
                }

                _response.IsSuccess = false;
                _response.Notification = "Mã OTP không chính xác";
                _response.Data = null;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        // ================== BÀI 5 – RESET PASSWORD ==================
        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Đặt lại mật khẩu thất bại";
                    _response.Data = removePasswordResult.Errors;
                    return BadRequest(_response);
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword);
                if (addPasswordResult.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đặt lại mật khẩu thành công";
                    _response.Data = null;
                    return Ok(_response);
                }

                _response.IsSuccess = false;
                _response.Notification = "Đặt lại mật khẩu thất bại";
                _response.Data = addPasswordResult.Errors;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        // ================== GET ALL RESULT BY USER ==================
[Authorize]
[HttpGet("GetAllResultByUser/{guid}")]
public async Task<IActionResult> GetAllResultByUser(string guid)
{
    try
    {
        // 1. Kiểm tra user tồn tại trong Identity
        var user = await _userManager.FindByIdAsync(guid);
        if (user == null)
        {
            _response.IsSuccess = false;
            _response.Notification = "Không tìm thấy user";
            return BadRequest(_response);
        }

        // 2. Lấy kết quả trực tiếp bằng GUID
        var data = await _db.LevelResults
            .Where(x => x.userId == guid)
            .Include(x => x.gameLevel)
            .ToListAsync();

        _response.IsSuccess = true;
        _response.Notification = "Lấy dữ liệu thành công";
        _response.Data = data;
        return Ok(_response);
    }
    catch (Exception ex)
    {
        _response.IsSuccess = false;
        _response.Notification = "Lỗi";
        _response.Data = ex.Message;
        return BadRequest(_response);
    }
}
        // ================== ADD TEST LEVEL ==================
        [HttpGet("AddTestLevel")]
        public IActionResult AddTestLevel()
        {
            _db.GameLevels.Add(new GameLevel
            {
                title = "Level Test",
                description = "Màn test từ API"
            });

            _db.SaveChanges();
            return Ok("Đã thêm Level");
        }

    }
}

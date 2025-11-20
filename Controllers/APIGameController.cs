using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;
using WebApplication1.DTO;
using Microsoft.EntityFrameworkCore;
using WebApplication1.ViewModel;
using WebApplication1.Services;
using System.IO;

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

        public APIGameController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _db = db;
            _userManager = userManager;
            _emailService = emailService;
            _response = new ResponseAPI();
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
                FullName = model.FullName
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
                var email = loginRequest.Email!;
                var password = loginRequest.Password!;

                var user = await _userManager.FindByEmailAsync(email);

                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đăng nhập thành công";
                    _response.Data = user;
                    return Ok(_response);
                }

                _response.IsSuccess = false;
                _response.Notification = "Đăng nhập thất bại";
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
        [HttpPost("SaveResult")]
        public async Task<IActionResult> SaveResult([FromBody] LevelResultDTO levelResult)
        {
            try
            {
                var levelResultSave = new LevelResult
                {
                    userId = int.Parse(levelResult.UserId),
                    levelId = levelResult.LevelId,
                    score = levelResult.Score,
                    completionDate = DateTime.Now
                };

                await _db.LevelResults.AddAsync(levelResultSave);
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
        [HttpGet("Rating/{idRegion}")]
        public async Task<IActionResult> Rating(int idRegion)
        {
            try
            {
                if (idRegion > 0)
                {
                    var nameRegion = await _db.Regions
                        .Where(x => x.regionId == idRegion)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();

                    if (nameRegion == null)
                    {
                        _response.IsSuccess = false;
                        _response.Notification = "Không tìm thấy khu vực";
                        _response.Data = null;
                        return BadRequest(_response);
                    }

                    var users = await _db.CustomUsers
                        .Where(x => x.regionId == idRegion)
                        .ToListAsync();

                    var results = await _db.LevelResults
                        .Where(x => users.Select(u => u.userId).Contains(x.userId))
                        .ToListAsync();

                    RatingVM ratingVM = new RatingVM
                    {
                        NameRegion = nameRegion,
                        userResultSums = new()
                    };

                    foreach (var item in users)
                    {
                        var sumScore = results
                            .Where(x => x.userId == item.userId)
                            .Sum(x => x.score);

                        var sumLevel = results
                            .Where(x => x.userId == item.userId)
                            .Count();

                        UserResultSum userResult = new UserResultSum
                        {
                            NameUser = item.username,
                            SumScore = sumScore,
                            SumLevel = sumLevel
                        };

                        ratingVM.userResultSums.Add(userResult);
                    }

                    _response.IsSuccess = true;
                    _response.Notification = "Lấy dữ liệu thành công";
                    _response.Data = ratingVM;
                    return Ok(_response);
                }
                else
                {
                    var users = await _db.CustomUsers.ToListAsync();
                    var results = await _db.LevelResults.ToListAsync();

                    RatingVM ratingVM = new RatingVM
                    {
                        NameRegion = "Tất cả",
                        userResultSums = new()
                    };

                    foreach (var item in users)
                    {
                        var sumScore = results
                            .Where(x => x.userId == item.userId)
                            .Sum(x => x.score);

                        var sumLevel = results
                            .Where(x => x.userId == item.userId)
                            .Count();

                        UserResultSum userResult = new UserResultSum
                        {
                            NameUser = item.username,
                            SumScore = sumScore,
                            SumLevel = sumLevel
                        };

                        ratingVM.userResultSums.Add(userResult);
                    }

                    _response.IsSuccess = true;
                    _response.Notification = "Lấy dữ liệu thành công";
                    _response.Data = ratingVM;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        // ================== BÀI 6 – GET USER INFORMATION ==================
        [HttpGet("GetUserInformation/{userId}")]
        public async Task<IActionResult> GetUserInformation(int userId)
        {
            try
            {
                var user = await _db.CustomUsers
                    .Where(x => x.userId == userId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                UserInformationVM userInfo = new UserInformationVM
                {
                    Name = user.username,
                    Email = "Không có Email (model không có trường Email)",
                    Avatar = user.linkAvatar ?? "",
                    Region = await _db.Regions
                        .Where(x => x.regionId == user.regionId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync() ?? "Không xác định"
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
                var user = await _db.CustomUsers
                    .Where(x => x.userId == int.Parse(userInformationDTO.UserId))
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                user.username = userInformationDTO.Name;
                user.regionId = userInformationDTO.RegionId;

                if (userInformationDTO.Avatar != null)
                {
                    var fileExtension = Path.GetExtension(userInformationDTO.Avatar.FileName);
                    var fileName = $"{userInformationDTO.UserId}{fileExtension}";

                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads/avatars",
                        fileName
                    );

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userInformationDTO.Avatar.CopyToAsync(stream);
                    }

                    user.linkAvatar = fileName;
                }

                await _db.SaveChangesAsync();

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
    }
}

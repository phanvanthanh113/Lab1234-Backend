using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class LoginVM
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string OTP { get; set; } = string.Empty;
    }
}

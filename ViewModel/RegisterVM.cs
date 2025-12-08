using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.ViewModel
{
    public class RegisterVM
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OTP { get; set; } = string.Empty;

        [Required]
        public string? Name { get; set; }

        public string? LinkAvatar { get; set; }

        [Required]
        public int RegionId { get; set; }

        public IEnumerable<SelectListItem>? Regions { get; set; }
    }
}

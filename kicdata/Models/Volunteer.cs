using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    public class Volunteer : Member
    {
        [Required]
        [Display(Name = "Fetlife Profile Name")]
        public string? FetName { get; set; }

        [Required]
        [Display(Name = @"Club425 ID (This was provided on registration.)")]
        public int? ClubId { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string? Email { get;set; }

        [Display(Name = @"Phone Number (Optional)")]
        public string? PhoneNumber { get; set;}

        [Display(Name = "Positions you are interested in working.")]
        public List<string>? Positions { get; set; }

        [Display(Name = "Anything else we should know?")]
        public string? Details { get; set; }
    }
}
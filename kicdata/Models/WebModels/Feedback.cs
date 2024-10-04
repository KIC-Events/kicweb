using System.ComponentModel.DataAnnotations;

namespace KiCData.Models.WebModels
{
    public class Feedback
    {
        [Required]
        [Display(Name="What would you like us to know?")]
        public string? Text { get; set; }

        [Display(Name = "Email Address (If you'd like a follow up.)")]
        public string? Email { get; set; }

        [Display(Name="Name (If you'd like a follow up.)")]
        public string? Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace KiCData.Models.WebModels
{
    public class Feedback
    {
        [Required]
        [Display(Name="What would you like us to know?")]
        public string? Text { get; set; }
    }
}

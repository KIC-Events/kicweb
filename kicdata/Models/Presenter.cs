using System.ComponentModel.DataAnnotations;

namespace KiCData.Models
{
    public class Presenter
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Your FetLife name. If none, put N/A.")]
        public string? FetName { get; set; }

        [Required]
        [Display(Name = "The name we should use for you or your business in promotional materials.")]
        public string? PublicName { get; set; }

        [Required]
        [Display(Name = "Your Email Address.")]
        public string? EmailAddress { get; set; }

        [Display(Name = "A short bio about you or your business.")]
        public string? Bio { get; set; }

        public DateOnly? LastAttended { get; set; }
    }
}
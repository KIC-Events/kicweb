using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    [Table("Presenters")]
    public class Presenter : Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? PresenterId { get; set; }

        [Required]
        [Display(Name = "The name we should use for you or your business in promotional materials.")]
        public string? PublicName { get; set; }

        [Display(Name = "A short bio about you or your business.")]
        public string? Bio { get; set; }

        public DateOnly? LastAttended { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    [Table("Presenters")]
    public class Presenter 
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        //public Guid? MemberId { get; set; }
        //int for testing purposes only
        public int? MemberId { get; set; }
        public virtual Member Member { get; set; }


        [Required]
        [Display(Name = "The name we should use for you or your business in promotional materials.")]
        public string? PublicName { get; set; }

        [Display(Name = "A short bio about you or your business.")]
        public string? Bio { get; set; }

        public DateOnly? LastAttended { get; set; }

        public string? Requests { get; set; }

        public decimal? Fee { get; set; }
        public string? Details { get; set; }

    }
}
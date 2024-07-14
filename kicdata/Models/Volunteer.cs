using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    [Table("Volunteers")]
    public class Volunteer : Member
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int? VolunteerId { get; set; }

        //public Guid? MemberId { get; set; }

        //public virtual Member Member { get; set; }

      
        [Display(Name = "Positions you are interested in working.")]
        public List<string>? Positions { get; set; }

        [Display(Name = "Anything else we should know?")]
        public string? Details { get; set; }
    }
}
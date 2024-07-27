using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    [Table("Volunteer")]
    public class Volunteer 
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        //public Guid? MemberId { get; set; }
        //int for testing purposes only
        public int? MemberId { get; set; }

        public virtual Member Member { get; set; }


        [Display(Name = "Positions you are interested in working.")]
        public List<string>? Positions { get; set; }

        [Display(Name = "Preferred Shift", Prompt = "Shifts you are available to work.")]
        public List<string>? Shifts { get; set; }

        [Display(Name = "Anything else we should know?")]
        public string? Details { get; set; }
    }
}
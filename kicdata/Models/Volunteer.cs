using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    public class Volunteer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Display(Name = "Positions you are interested in working.")]
        public List<string>? Positions { get; set; }

        [Display(Name = "Anything else we should know?")]
        public string? Details { get; set; }

        public bool? IsStaff { get; set; }

        public int MemberId { get; set; }

        public virtual Member Member { get; private set; }

        /*
         * public Member FormMember { get; set; }
         * public void CheckMember()
        {
            if (FormMember.ClubID != null)
            {
                //Check db for matching member and get ID
                //Set Member from ID
            }
        }
        */
    }
}
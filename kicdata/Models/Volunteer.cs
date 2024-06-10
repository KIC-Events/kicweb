using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    public class Volunteer
    {
        [Display(Name = "Positions you are interested in working.")]
        public List<string>? Positions { get; set; }

        [Display(Name = "Anything else we should know?")]
        public string? Details { get; set; }

        public Member FormMember { get; set; }

        public Member Member { get; private set; }

        public void CheckMember()
        {
            if (FormMember.ClubID != null)
            {
                //Check db for matching member and get ID
                //Set Member from ID
            }
        }
    }
}
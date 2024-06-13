using System.ComponentModel.DataAnnotations;

namespace KiCData.Models
{
    public class Presenter
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "The name we should use for you or your business in promotional materials.")]
        public string? PublicName { get; set; }

        [Display(Name = "A short bio about you or your business.")]
        public string? Bio { get; set; }

        //public Member? FormMember { get; set; }

        public int MemberId { get; set; }

        public virtual Member? Member { get; private set; }

        public DateOnly? LastAttended { get; set; }

        public virtual List<Presentation> Presentations { get; set; }
        /*
        public void CheckMember()
        {
            if(FormMember.ClubID != null)
            {
                //Check db for matching member and get ID
                //Set Member from ID
            }
        }
        */

    }
}
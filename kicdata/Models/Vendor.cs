using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class Vendor

    {
        [Required]
        [Display(Name  = "Your business or professional name.")]
        public string? PublicName { get; set; }

        [Required]
        [Display(Name = "About your business.")]
        public string? Bio { get; set; }

        public int MemberId { get; set; }

        public virtual Member Member { get; private set; }

        /*
        public Member FormMember { get; set; }

        public Member Member { get; private set; }

        public void CheckMember()
        {
            if (FormMember.ClubId != null)
            {
                //Check db for matching member and get ID
                //Set Member from ID
            }
        }
        */
    }
}

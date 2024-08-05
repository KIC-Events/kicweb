using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class VolunteerViewModel : MemberViewModel
    {
        [Required(ErrorMessage = "Please select at least one position you would like to volunteer for.")]
        [Display(Name = "Position", Prompt ="Please select the positions you would like to volunteer for.")]
        public List<string>? Positions { get; set; }

        [Required(ErrorMessage = "Please select at least one shift you are available to volunteer.")]
        [Display(Name = "Shift", Prompt = "Please select the shifts you are available to volunteer.")]
        public List<string>? Shifts { get; set; }

        [Display(Name = "Additional Information", Prompt = "Please provide any additional information you would like us to know.")]
        public string? Details{ get; set; }

        public VolunteerViewModel(string firstname, string lastname, string email, DateOnly dateofbirth, string fetname, int clubid, string phonenumber, string additionalinfo, List<string> positions, List<string> shifts, string details)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            DateOfBirth = dateofbirth;
            FetName = fetname;
            ClubId = clubid;
            PhoneNumber = phonenumber;
            AdditionalInfo = additionalinfo;
            Positions = positions;
            Shifts = shifts;
            Details = details;
        }

        public VolunteerViewModel()
        {
            
        }
    }
}

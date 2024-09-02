using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class AttendeeViewModel : MemberViewModel

    {
        [Required(ErrorMessage = "Please provide the name you would like on your event badge.")]
        [Display(Name = "Badge Name")]
        public string? BadgeName { get; set; }

        public AttendeeViewModel(string firstname, string lastname, string email, DateOnly dateofbirth, string fetname, int clubid, string phonenumber, string additionalinfo, string badgename)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            DateOfBirth = dateofbirth;
            FetName = fetname;
            ClubId = clubid;
            PhoneNumber = phonenumber;
            AdditionalInfo = additionalinfo;
            BadgeName = badgename;
        }

        public AttendeeViewModel()
        {
            
        }


    }

    public class ConfirmationViewModel
    {
        [Display(Name ="First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Date of Birth")]
        public DateOnly DateofBirth { get; set; }

        [Display(Name = "Badge Name")]
        public string? BadgeName { get; set; }
    }
}

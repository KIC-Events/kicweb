using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class MemberViewModel
    {

        [Required(ErrorMessage = "Please enter your legal first name.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your legal last name.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your birthday")]
        [Display(Name = "Date of Birth")]
        public DateOnly? DateOfBirth { get; set; }

        [Display(Name = "Fetlife Profile Name", Prompt = "Enter your fetlife profile name, if available")]
        public string? FetName { get; set; }

        [Display(Name = @"Club425 ID (This was provided on registration.)")]
        public int? ClubId { get; set; }

        [Display(Name = @"Phone Number (Optional)")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Additional Information")]
        public string? AdditionalInfo { get; set; }


    }

    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Please enter Registrant's legal first name.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Registant's legal last name.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter Date of Birth")]
        [Display(Name = "Date of Birth")]
        public DateOnly DateofBirth { get; set; }
    }
}

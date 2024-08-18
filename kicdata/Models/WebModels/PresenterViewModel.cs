using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class PresenterViewModel : MemberViewModel
    {

        [Required(ErrorMessage = "Please enter the name we should use for you or your business in promotional material.")]
        [Display(Name = "Presenter Name")]
        public string? PublicName { get; set; }

        [Required(ErrorMessage = "Please enter a short bio for promotional material.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        [Display(Name = "Special Requests", Prompt = "Please enter any special requests you have.")]
        public string? Requests { get; set; }

        [Display(Name = "Fee", Prompt = "Please enter your fee.")]
        public decimal? Fee { get; set; }

        [Display(Name = "Details", Prompt = "Please enter any additional details.")]
        public string? Details { get; set; }

        public PresenterViewModel(string firstname, string lastname, string email, DateOnly dateofbirth, string fetname, int clubid, string phonenumber, string additionalinfo, string publicname, string bio, string requests, decimal fee, string details)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            DateOfBirth = dateofbirth;
            FetName = fetname;
            ClubId = clubid;
            PhoneNumber = phonenumber;
            AdditionalInfo = additionalinfo;
            PublicName = publicname;
            Bio = bio;
            Requests = requests;
            Fee = fee;
            Details = details;
        }

        public PresenterViewModel()
        {
            
        }
    }
}

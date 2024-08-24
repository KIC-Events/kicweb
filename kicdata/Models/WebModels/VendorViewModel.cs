using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    internal class VendorViewModel : MemberViewModel
    {
        [Required(ErrorMessage = "Please provide your business or professional name.")]
        [Display(Name = "Public Name")]
        public string? PublicName { get; set; }

        [Required(ErrorMessage = "Please provide a description of your business.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        [Display(Name = "Last Attended")]
        public DateTime? LastAttended { get; set; }

        [Required(ErrorMessage = "Please provide the type of merchandise you sell.")]
        [Display(Name = "Merchandise Type")]
        public string? MerchType { get; set; }

        [Required(ErrorMessage = "Please provide the minimum price of your merchandise.")]
        [Display(Name = "Minimum Price")]
        public decimal? PriceMin { get; set; }

        [Required(ErrorMessage = "Please provide the maximum price of your merchandise.")]
        [Display(Name = "Maximum Price")]
        public decimal? PriceMax { get; set; }

        [Required(ErrorMessage = "Please provide the average price of your merchandise.")]
        [Display(Name = "Average Price")]
        public decimal? PriceAvg { get; set; }

        public VendorViewModel(string firstname, string lastname, string email, DateOnly dateofbirth, string fetname, int clubid, string phonenumber, string additionalinfo, string publicname, string bio, DateTime lastattended, string merchtype, decimal minprice, decimal maxprice, decimal avgprice)
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
            LastAttended = lastattended;
            MerchType = merchtype;
            PriceMin = minprice;
            PriceMax = maxprice;
            PriceAvg = avgprice;
        }

        public VendorViewModel()
        {
            
        }


    }
}

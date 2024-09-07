using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace KiCData.Models
{
    [Table("Member")]
    public class Member
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Guid? Id { get; set; }
        // int for testing purposes only
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Legal First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Legal Last Name")]
        public string? LastName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Birthday")]
        public DateOnly? DateOfBirth { get; set; }

        [Display(Name = "Fetlife Profile Name")]
        public string? FetName { get; set; }

        [Display(Name = @"Club425 ID (This was provided on registration.)")]
        public int? ClubId { get; set; }

        [Display(Name = @"Phone Number (Optional)")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Member ID")]
        public int? PublicId { get; set; }

        [Display(Name = "Additional Information")]
        public string? AdditionalInfo { get; set; }

        public int? VendorId { get; set; }

        public Vendor? Vendor { get; set; }

        public Volunteer? Volunteer { get; set; }
        public Staff? Staff { get; set; }

        public int? PresenterId { get; set; }
        public Presenter? Presenter { get; set; }

        public User? User { get; set; }
        public Attendee? Attendee { get; set; }

        public string? SexOnID { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
       

        [NotMapped]
        [Display(Name = "Legal Name")]
        public string LegalName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }



    }
}
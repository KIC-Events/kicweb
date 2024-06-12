using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        
        [Required]
        [Display(Name = "Legal First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Legal Last Name")]
        public string? LastName { get; set; }

        [Required]
        [Display(Name = "Club ID #")]
        public string? ClubId { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        public bool? IsVendor { get; set; }

        public bool? IsVolunteer { get; set; }

        public bool? IsPresenter { get; set; }

        public bool? IsStaff {  get; set; }

        [Display(Name = "Optional: Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Your FetLife name. If none, put N/A.")]
        public string? FetName { get; set; }

        public DateOnly? LastAttended { get; private set; }

        [Display(Name = "Additional Info")]
        public string? AdditionalInfo { get; set; }

        [NotMapped]
        [Display(Name = "Legal Name")]
        public string LegalName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public void UpdateLastAttended(DateOnly date)
        {
            LastAttended = date;
        }
    }
}
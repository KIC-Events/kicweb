using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    public abstract class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Legal First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Legal Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsVendor { get; set; }

        public bool IsVolunteer { get; set; }

        public bool IsPresenter { get; set; }

        public bool IsStaff {  get; set; }

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



    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    public class Volunteer : Member
    {
        [Required]
        public string FetName { get; set; }

        public List<string>? Positions { get; set; }

        public string? Details { get; set; }

        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public Volunteer(string fetname)
        {
            FetName = fetname;
        }
    }
}
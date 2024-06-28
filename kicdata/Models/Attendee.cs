using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    [Table("Attendees")]
    public class Attendee : Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendeeId { get; set; }

        [Required]
        [Display(Name = "Sex listed on Government ID")]
        public string Sex { get; set; }

        [Required]
        [Display(Name = "Type of Ticket")]
        public string TicketType { get; set; }

        [Required]
        [Display(Name = "Preferred Badge Name")]       
        public string BadgeName { get; set; }
    }
        
}
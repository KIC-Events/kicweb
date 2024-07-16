using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    [Table("Attendees")]
    public class Attendee 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public Guid? MemberId { get; set; }

        public virtual Member Member { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Required]
        [Display(Name = "Preferred Badge Name")]       
        public string BadgeName { get; set; }

        public bool BackgroundChecked { get; set; }

        public int ConfirmationNumber { get; set; }
        public bool WaitListed { get; set; }
        public string? RoomPreference { get; set; }

        public bool IsPaid { get; set; }
    }
        
}
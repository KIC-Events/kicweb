using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        //public Guid? EventId { get; set; }
        //int for testing purposes only
        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }

        public double Price { get; set; }

        public string? Type { get; set; }

        public DateOnly? DatePurchased { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public bool? IsComped { get; set; }

        public virtual Attendee? Attendee { get; set; }
    }

    public class TicketComp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        public string? DiscountCode { get; set; }
            
        public int? TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }

        public double? CompAmount { get; set; }

        public string? CompReason { get; set; }

        public string? AuthorizingUser { get; set; }

        public double? CompPct { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class TicketComp
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public int? TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; } 

        public decimal? CompAmount { get; set; }

        public string? CompReason { get; set; }

        public string AuthorizingUser { get; set; }
        
    }
}

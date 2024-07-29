using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    [Table("EventVolunteer")]
    public class EventVolunteer
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public int? VolunteerId { get; set; }
        public virtual Volunteer? Volunteer { get; set; }

        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }

        public int? ShiftNumber { get; set; }

        public string? Position { get; set; }
    }
}

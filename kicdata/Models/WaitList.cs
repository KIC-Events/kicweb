using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class WaitList

    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int AttendeeId { get; set; }
        public virtual Attendee Attendee { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string? Comments { get; set; }
    }
}

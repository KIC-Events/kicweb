using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class PendingVolunteer
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int VolunteerID { get; set; }
        [Required]
        public int EventId {  get; set; }
        [Required]
        public int? PreferredShift { get; set; }
        [Required]
        public string? PreferredPositions { get; set; }


    }
}

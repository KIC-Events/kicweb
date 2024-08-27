using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class Staff
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        [Required]
        //public Guid? MemberId { get; set; }
        //int for testing purposes only
        public int? MemberId { get; set; }
        public virtual Member? Member { get; set; }
        [Required]
        public string? Position { get; set; }
    }

    
    
}

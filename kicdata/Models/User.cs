using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class User : Member
    {
        [Key]
        public Guid? Id { get; set; }

        public Guid? MemberId { get; set; }
        public virtual Member? Member { get; set; }

        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }

    }
}

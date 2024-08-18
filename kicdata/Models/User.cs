using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        //public Guid? MemberId { get; set; }
        //int for testing purposes only
        public int? MemberId { get; set; }
        public virtual Member? Member { get; set; }

        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }

        public virtual ICollection<Group>? Groups { get; set; }

        public string? Token { get; set; }

    }
}

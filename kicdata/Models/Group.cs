using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}

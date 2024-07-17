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
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        //public Guid? EventId { get; set; }
        //int for testing purposes only
        public int? EventId { get; set; }
        public virtual Event Event { get; set; }

        public decimal Price { get; set; }

        public string? Type { get; set; }

        public string? Name { get; set; }

        public DateOnly? DatePurchased { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public bool? IsComped { get; set; }


    }
}

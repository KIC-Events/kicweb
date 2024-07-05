using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    [Table("Vendors")]
    public class Vendor : Member

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? VendorId { get; set; }

        [Required]
        [Display(Name  = "Your business or professional name.")]
        public string? PublicName { get; set; }

        [Required]
        [Display(Name = "About your business.")]
        public string? Bio { get; set; }

        public DateOnly? LastAttended { get; set; }
    }
}

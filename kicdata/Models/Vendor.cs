using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    [Table("Vendor")]
    public class Vendor 

    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        //public Guid? MemberId { get; set; }
        //int for testing purposes only
        

        [Required]
        [Display(Name  = "Your business or professional name.")]
        public string? PublicName { get; set; }

        [Required]
        [Display(Name = "About your business.")]
        public string? Bio { get; set; }

        public DateOnly? LastAttended { get; set; }

        public string? MerchType { get; set; }

        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public decimal? PriceAvg { get; set; }

        public string? ImgPath { get; set; }

        public ICollection<Member> Members { get; set; } = new List<Member>();

    }
    
}

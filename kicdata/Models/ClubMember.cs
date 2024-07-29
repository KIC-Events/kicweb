using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    [Table("ClubMember")]
    public class ClubMember
    {
        [Key]
        public Guid? Id { get; set; }
        [Required]
        public int ClubId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateOnly? DateOfBirth { get; set; }
        [Required]  
        public string? Sex { get; set; }
    }
}

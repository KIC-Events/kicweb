using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiCData.Models
{
    [Table("Volunteer")]
    public class Volunteer 
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        //public Guid? MemberId { get; set; }
        //int for testing purposes only
        public int? MemberId { get; set; }

        public virtual Member Member { get; set; }


        [Display(Name = "Positions you are interested in working.")]
        public List<string>? Positions { get; set; }

        [Display(Name = "Anything else we should know?")]
        public string? Details { get; set; }
    }

    public class PendingVolunteer
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int? EventId { get; set; }

        [Required]
        public string? PreferredPositions { get; set; }
        public virtual Event? Event { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public PendingVolunteer() { }

        public PendingVolunteer(int volunteerID, int eventId, string? preferredPositions)
        {
            EventId = eventId;
            PreferredPositions = preferredPositions;
        }


    }
}
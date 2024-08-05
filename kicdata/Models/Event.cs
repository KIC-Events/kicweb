using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KiCData.Models
{
    [Table("Event")]
    public class Event
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Guid? Id { get; set; }  
        //int for testing purposes only
        public int? Id { get; set; }

        public string? Name { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
        
        public string? Description { get; set; } 
        
        public string? Topic { get; set; }

        public int? VenueId { get; set; }
        public virtual Venue? Venue { get; set; }

        public string? ImagePath { get; set; }

        public string? Link { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KiCData.Models
{
    public class Event
    {
        [Key]
        public Guid? EventId { get; set; }  

        public string? Name { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
        
        public string? Description { get; set; } 
        
        public string? Topic { get; set; }

        public int? VenueId { get; set; }
        public virtual Venue? Venue { get; set; }

        public string? ImagePath { get; set; }



    }
}
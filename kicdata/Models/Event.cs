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

    public class EventStaff
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        [Required]
        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }
        [Required]
        public int? StaffId { get; set; }
        public virtual Staff? Staff { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public string? Role { get; set; }
    }

    [Table("EventVendor")]
    public class EventVendor
    {
        [Key]
        public int? Id { get; set; }

        public int VendorId { get; set; }
        public virtual Vendor? Vendor { get; set; }

        public int EventId { get; set; }
        public virtual Event? Event { get; set; }


        public bool IsPaid { get; set; }

        public int ConfirmationNumber { get; set; }

        //public string VendorSpace { get; set; }

    }

    [Table("EventVolunteer")]
    public class EventVolunteer
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public int? VolunteerId { get; set; }
        public virtual Volunteer? Volunteer { get; set; }

        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public string? Position { get; set; }

        
    }
}
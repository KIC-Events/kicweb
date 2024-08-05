using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class EventViewModel
    {
        [Required(ErrorMessage = "Please provide a name for this event.")]
        [Display(Name = "Event Name")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Please provide a start date for this event.")]
        [Display(Name = "Start Date")]
        public DateOnly? StartDate { get; set; }
        
        [Required(ErrorMessage = "Please provide an end date for this event.")]
        [Display(Name = "End Date")]
        public DateOnly? EndDate { get; set; }
        
        [Required(ErrorMessage = "Please provide a topic for this event.")]
        [Display(Name = "Topic")]
        public string? Topic { get; set; }

        [Required(ErrorMessage = "Please provide a description for this event.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Please Select a venue for this event.")]
        [Display(Name = "Venue")]
        public int? VenueId { get; set; }

        //Lists all venues for the user to select from
        public List<SelectListItem>? Venues { get; set; }


        public EventViewModel(string name, DateOnly startDate, DateOnly endDate, string topic, string description, int venueId)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Topic = topic;
            Description = description;
            VenueId = venueId;
        }

        public EventViewModel()
        {
            
        }
    }
    public class EventVendorViewModel 
    {
        [Required(ErrorMessage ="Please Select the event.")]
        [Display(Name ="Event")]
        public int? EventId { get; set; }
        public IEnumerable<Event>? Events { get; set; }

        [Required(ErrorMessage ="Please Select the vendor.")]
        [Display(Name ="Vendor")]
        public int? VendorId { get; set; }
        public IEnumerable<Vendor>? Vendors { get; set; }

        public bool IsPaid { get; set; } = false;

        public int? ConfirmationNumber { get; set; }

    }

    public class EventVolunteerViewModel
    {
        [Required(ErrorMessage = "Please Select the event.")]
        [Display(Name = "Event")]
        public int? EventId { get; set; }
        public IEnumerable<Event>? Events { get; set; }

        [Required(ErrorMessage = "Please Select the volunteer.")]
        [Display(Name = "Volunteer")]
        public int? VolunteerId { get; set; }
        public IEnumerable<Volunteer>? Volunteers { get; set; }

        public int? ShiftNumber { get; set; }

        public string? Position { get; set; }
    }

}

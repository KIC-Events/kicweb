using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class VenueViewModel
    {
        [Required(ErrorMessage = "Please provide a name for this venue.")]
        [Display(Name = "Venue Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please provide an address for this venue.")]
        [Display(Name = "Street Address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please provide a city for this venue.")]
        [Display(Name = "City")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please provide a state for this venue.")]
        [Display(Name = "State")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Please provide a capacity for this venue.")]
        [Display(Name = "Capacity")]
        public int? Capacity { get; set; }

        [Display(Name ="Venue Cost",Prompt ="Please enter cost for this venue.")]
        public float? Cost { get; set; }


        public VenueViewModel(string name, string address, string city, string state, int capacity, float? cost)
        {
            Name = name;
            Address = address;
            City = city;
            State = state;
            Capacity = capacity;
            Cost = cost;
        }

        public VenueViewModel()
        {
            
        }
    }
}

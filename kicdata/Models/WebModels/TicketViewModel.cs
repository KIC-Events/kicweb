using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class TicketViewModel
    {
        [Required(ErrorMessage = "Please select an event for this ticket.")]
        [Display(Name = "Event")]
        public int? EventId { get; set; }

        //Lists all events for the user to select from
        public IEnumerable<Event>? Events { get; set; }

        [Required(ErrorMessage = "Please provide a price for this ticket.")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please selet the ticket type.")]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "Please select the date purchased.")]
        [Display(Name = "Date Purchased")]
        public DateTime? DatePurchased { get; set; }

        [Required(ErrorMessage = "Please select the start date.")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Please select the end date.")]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Please select if this ticket is comped.")]
        [Display(Name = "Comped")]
        public bool? IsComped { get; set; }

        public TicketViewModel(int eventId, decimal price, string type, DateTime datePurchased, DateTime startDate, DateTime endDate, bool isComped)
        {
            EventId = eventId;
            Price = price;
            Type = type;
            DatePurchased = datePurchased;
            StartDate = startDate;
            EndDate = endDate;
            IsComped = isComped;
        }

        public TicketViewModel()
        {
            
        }

    }
}

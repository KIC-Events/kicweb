using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Rendering;

namespace KiCData.Models.WebModels
{
    public class TicketViewModel
    {
        [Required(ErrorMessage = "Please select an event for this ticket.")]
        [Display(Name = "Event")]
        public int? EventId { get; set; }

        //Lists all events for the user to select from
        public List<SelectListItem>? Events { get; set; }

        [Required(ErrorMessage = "Enter quantity of tickets")]
        public int QtyTickets { get; set; }

        [Required(ErrorMessage = "Please provide a price for this ticket.")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Please select the ticket type.")]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        
        [Required(ErrorMessage = "Please select the start date.")]
        [Display(Name = "Start Date")]
        public DateOnly? StartDate { get; set; }

        [Required(ErrorMessage = "Please select the end date.")]
        [Display(Name = "End Date")]
        public DateOnly? EndDate { get; set; }

        

        public TicketViewModel(int eventId, double price, string type, int qtyTickets, DateOnly startDate, DateOnly endDate)
        {
            EventId = eventId;
            QtyTickets = qtyTickets;
            Price = price;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            
        }

        public TicketViewModel()
        {
            
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class PresentationViewModel
    {
        [Required(ErrorMessage = "Please provide a name for this presentation.")]
        [Display(Name = "Presentation Name")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Please provide a description for this presentation.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please select the type of presentation.")]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "Please select the presenter.")]
        [Display(Name = "Presenter")]
        public int? PresenterId { get; set; }
        
        //Lists all presenters for the user to select from
        public List<Presenter>? Presenters { get; set; }

        [Required(ErrorMessage = "Please select the event.")]
        [Display(Name = "Event")]
        public int? EventId { get; set; }

        //Lists all events for the user to select from
        public List<Event>? Events { get; set; }

        public PresentationViewModel(string name, string description, string type, int presenterid, int eventid) 
        {
            Name = name;
            Description = description;
            Type = type;
            PresenterId = presenterid;
            EventId = eventid;
        }

        public PresentationViewModel()
        {
            
        }
    }
}

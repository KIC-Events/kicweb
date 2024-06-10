using KiCData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class Presentation
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "The name of your class or presentation.")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "A short description of your class or presentation.")]
        public string? Description { get; set; }

        [Display(Name = "To what kink or kinks does your class pertain?")]
        public string? Type { get; set; }

        public Presenter? Presenter { get; set; }

        public Event Event { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace KiCData.Models
{
    public class Event
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        public DateOnly? Date { get; set; }

        public Location Location { get; set; }

        public void SetLocation(string name)
        {
            //Add db query to set loction via name call
        }
    }
}
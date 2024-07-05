using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KiCData.Models
{
    public class Event
    {
        [Key]
        public Guid? EventId { get; set; }  

    }
}
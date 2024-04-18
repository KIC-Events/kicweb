using KiCData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kicdata.Models
{
    public class Presentation
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Type { get; set; }

        public Presenter? Presenter { get; set; }

        public DateOnly? Date { get; set; }
    }
}

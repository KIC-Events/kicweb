using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models
{
    public class EventVendor
    {
        public int? Id { get; set; }

        public int VendorId { get; set; }
        public virtual Vendor? Vendor { get; set; }

        public int EventId { get; set; }
        public virtual Event? Event { get; set; }


        public bool IsPaid { get; set; }

        public int ConfirmationNumber { get; set; }
    }
}

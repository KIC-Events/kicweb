using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Models.WebModels
{
    public class PaymentViewModel
    {
        public RegistrationViewModel Registration { get; set; }

        public string CCNumber { get; set; }
    }
}

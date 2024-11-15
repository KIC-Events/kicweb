using KiCData.Models.WebModels;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Services
{
    public interface IPaymentService
    {
        public int CheckInventory(string objectSearchTerm, string variationSearchTerm);
        public PaymentLink CreateCurePaymentLink(List<RegistrationViewModel> regList);

        public PaymentLink CreatePaymentLink(List<RegistrationViewModel> regList, KiCData.Models.Event kicEvent, string[] discountCodes = null, string redirectUrl = null);
    }
}

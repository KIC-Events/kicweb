using KiCData.Models.WebModels;
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
        public string CreatePaymentLink(List<RegistrationViewModel> regList);
    }
}

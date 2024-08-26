using Microsoft.Extensions.Configuration;
using Square;
using Square.Authentication;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Services
{
    public class PaymentService : IPaymentService
    {
        private SquareClient _client;

        public PaymentService(IConfigurationRoot configuration)
        {
            Square.Environment env = Square.Environment.Production;

            if (configuration["Square:Environment"] == "Sandbox")
            {
                env = Square.Environment.Sandbox;
            }

            _client = new SquareClient.Builder().BearerAuthCredentials
                (
                    new BearerAuthModel.Builder(configuration["Square:Token"])
                    .Build()
                )
                .Environment(env)
                .Build();
        }

        public int CheckInventory(string objectSearchTerm)
        {
            int response = checkInventory(objectSearchTerm);
            return response;
        }

        private int checkInventory(string objectSearchTerm)
        {
            ListCatalogResponse catResponse = _client.CatalogApi.ListCatalog();
            CatalogObject obj = catResponse.Objects
                .Where(o => o.ItemData.Name.Contains(objectSearchTerm))
                .FirstOrDefault();

            if (obj == null)
            {
                throw new Exception("Object not found.");
            }

            RetrieveInventoryCountResponse countResponse = _client.InventoryApi.RetrieveInventoryCount(obj.Id);

            if (countResponse == null)
            {
                throw new Exception("No count for " + objectSearchTerm + " found.");
            }

            if(countResponse.Counts.Count > 1)
            {
                throw new Exception("Found multiple counts for " + objectSearchTerm + ".");
            }

            InventoryCount count = countResponse.Counts.FirstOrDefault();

            int response = int.Parse(count.Quantity);

            return response;
        }
    }
}

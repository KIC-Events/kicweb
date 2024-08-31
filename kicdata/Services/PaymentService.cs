using Microsoft.Extensions.Configuration;
using Square;
using Square.Authentication;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public int CheckInventory(string objectSearchTerm, string variationSearchTerm)
        {
            try
            {
                int response = checkInventory(objectSearchTerm, variationSearchTerm);
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        private int checkInventory(string objectSearchTerm, string variationSearchTerm)
        {
            ListCatalogResponse catResponse = _client.CatalogApi.ListCatalog();
            CatalogObject obj = catResponse.Objects
                .Where(o => o.ItemData.Name.Contains(objectSearchTerm))
                .FirstOrDefault();

            if (obj == null)
            {
                throw new Exception("Object not found.");
            }

            string varId = obj.ItemData.Variations
                .Where(v => v.ItemVariationData.Name.Contains(variationSearchTerm))
                .FirstOrDefault()
                .Id;

            RetrieveInventoryCountResponse countResponse = _client.InventoryApi.RetrieveInventoryCount(varId);

            if (countResponse.Counts == null)
            {
                throw new Exception("No count for " + variationSearchTerm + " found.");
            }

            if(countResponse.Counts.Count > 1)
            {
                throw new Exception("Found multiple counts for " + variationSearchTerm + ".");
            }

            InventoryCount count = countResponse.Counts.FirstOrDefault();

            int response = int.Parse(count.Quantity);

            return response;
        }

        public string CreatePaymentLink()
        {
            string paymentLink = createPaymentLink();

            return paymentLink;
        }

        private string createPaymentLink()
        {
            var catalog = _client.CatalogApi.ListCatalog();
            var catObj = catalog.Objects
                .Where(o => o.ItemData.Name.Contains("CURE"))
                .FirstOrDefault();
            string id = catObj.Id;

            Order order = new Order.Builder("temp")
                .LineItems(new List<OrderLineItem>
                {
                    new OrderLineItem.Builder("1")
                    .CatalogObjectId(id)
                    .Build()
                })
                .Build();
            CreateOrderRequest orderRequest = new CreateOrderRequest.Builder()
                .IdempotencyKey(Guid.NewGuid().ToString())
                .Order(order)
                .Build();

            var orderResponse = _client.OrdersApi.CreateOrder(orderRequest).Order;

            CreatePaymentLinkRequest paymentRequest = new CreatePaymentLinkRequest.Builder()
                .Order(order)
                .Build();

            CreatePaymentLinkResponse response = _client.CheckoutApi.CreatePaymentLink(paymentRequest);

            string paymentLink;

            return paymentLink;
        }
    }
}

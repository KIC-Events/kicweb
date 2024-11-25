using KiCData.Models;
using KiCData.Models.WebModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Services
{
    public class PaymentService : IPaymentService
    {
        private SquareClient _client;
        private IKiCLogger _logger;

        public PaymentService(IConfigurationRoot configuration, IKiCLogger logger)
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
            _logger = logger;
        }

        public int CheckInventory(string objectSearchTerm, string variationSearchTerm)
        {
            int response = checkInventory(objectSearchTerm, variationSearchTerm);
            return response;
        }

        private int checkInventory(string objectSearchTerm, string variationSearchTerm)
        {
            ListCatalogResponse catResponse = _client.CatalogApi.ListCatalog();
            CatalogObject? obj = catResponse.Objects
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

        /*
         * 10-24-2024 194-add-ticket-purchase-for-blashphemy
         * https://github.com/Malechus/kic/issues/194
         * This method should be a reusable method with injectable data
         * but since we are two and half months away from the event
         * rather than risk a refactor I am just renaming this method
         * from CreatePaymentLink to CreateCurePaymentLink and building a new
         * reusable CreatePaymentLink method.
         * Malechus
         */
        public PaymentLink CreateCurePaymentLink(List<RegistrationViewModel> regList)
        {
            PaymentLink paymentLink = createCurePaymentLink(regList);

            return paymentLink;
        }

        private PaymentLink createCurePaymentLink(List<RegistrationViewModel> regList)
        {
            List<OrderLineItem> orderLineItems = new List<OrderLineItem>();
            List<OrderLineItemDiscount> orderDiscounts = new List<OrderLineItemDiscount>();

            var locations = _client.LocationsApi.ListLocations();
            string locationID = locations.Locations.FirstOrDefault().Id;

            foreach (RegistrationViewModel reg in regList)
            {
                ListCatalogResponse catalog = _client.CatalogApi.ListCatalog();
                CatalogObject catObj = catalog.Objects
                    .Where(o => o.ItemData.Name == "CURE Event Ticket")
                    .FirstOrDefault();
                string id = catObj.Id;
                CatalogObject variation = catObj.ItemData.Variations.Where(v => v.ItemVariationData.Name == reg.TicketType).FirstOrDefault();
                string varId = variation.Id;

                var appliedDiscounts = new List<OrderLineItemAppliedDiscount>();

                if(reg.DiscountCode != null)
                {
                    string discountName = "";
                    switch (reg.TicketComp.CompReason)
                    {
                        case "Event Staff or Volunteer":
                            discountName = "Staff Comp";
                            break;
                        case "Scholarship":
                            discountName = "Staff Comp";
                            break;
                        case "Key Volunteer":
                            discountName = "Staff Comp";
                            break;
                        case "Comp - Gratuity":
                            discountName = "Staff Comp";
                            break;
                        case "Club 425 Member Discount":
                            discountName = "Club 425 Member";
                            break;
                        case "425 Early Access":
                            discountName = "Club 425 Member";
                            break;
                        default:
                            throw new Exception("Bad discount code.");
                            break;
                    }

                    var discount = catalog.Objects.Where(o => o.Type == "DISCOUNT" && o.DiscountData.Name == discountName).FirstOrDefault();

                    OrderLineItemAppliedDiscount orderLineItemAppliedDiscount = new OrderLineItemAppliedDiscount.Builder(discountUid: discount.Id)
                        .Build();

                    OrderLineItemDiscount lineItemDiscount = new OrderLineItemDiscount.Builder()
                        .Uid(discount.Id)
                        .Name(discount.DiscountData.Name)
                        .Scope("LINE_ITEM")
                        .AppliedMoney(discount.DiscountData.AmountMoney)
                        .AmountMoney(discount.DiscountData.AmountMoney)
                        .Build();

                    orderDiscounts.Add(lineItemDiscount);

                    appliedDiscounts.Add(orderLineItemAppliedDiscount);
                }

                OrderLineItem orderLineItem = new OrderLineItem.Builder(quantity: "1")
                    .CatalogObjectId(varId)
                    .AppliedDiscounts(appliedDiscounts)
                    .Note(reg.FirstName + " " + reg.LastName)
                    .Build();

                orderLineItems.Add(orderLineItem);
            }

            OrderServiceCharge orderServiceCharge = new OrderServiceCharge.Builder()
                .Name("Handling Fee")
                .Percentage("3")
                .CalculationPhase("SUBTOTAL_PHASE")
                .Build();

            List<OrderServiceCharge> serviceCharges = new List<OrderServiceCharge>();
            serviceCharges.Add(orderServiceCharge);

            OrderPricingOptions pricingOptions = new OrderPricingOptions.Builder()
                .AutoApplyTaxes(true)
                .Build();

            Order order = new Order.Builder(locationId: locationID)
                .LineItems(orderLineItems)
                .PricingOptions(pricingOptions)
                .ServiceCharges(serviceCharges)
                .Discounts(orderDiscounts)
                .Build();

            //CreateOrderRequest orderRequest = new CreateOrderRequest.Builder()
            //    .IdempotencyKey(Guid.NewGuid().ToString())
            //    .Order(order)
            //    .Build();
            //
            //CreateOrderResponse orderResponse = _client.OrdersApi.CreateOrder(orderRequest);

            CheckoutOptions options = new CheckoutOptions.Builder()
                .RedirectUrl("https://cure.kicevents.com/RegistrationSuccessful")
                .Build();

            CreatePaymentLinkRequest paymentRequest = new CreatePaymentLinkRequest.Builder()
                .IdempotencyKey(Guid.NewGuid().ToString())
                .Order(order)
                .CheckoutOptions(options)
                .Build();

            PaymentLink paymentLink;

            try
            {
                CreatePaymentLinkResponse response = _client.CheckoutApi.CreatePaymentLink(paymentRequest);

                paymentLink = response.PaymentLink;
            }
            catch(Square.Exceptions.ApiException ex)
            {
                _logger.LogSquareEx(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw ex;
            }

            return paymentLink;
        }

        /// <summary>
        /// Generates a dynamic Square payment link for the requested items.
        /// </summary>
        /// <param name="regList">List<RegistrationViewModel> containing the registrants purchasing event tickets.</param>
        /// <param name="kicEvent">The KiCData.Models.Event object for which tickets are being purchased.</param>
        /// <param name="discountCodes">String[] array of discount codes that could apply.</param>
        /// <param name="redirectUrl">Url for redirect after payment complete (merch, etc.) leave empty for generic success page.</param>
        /// <returns>PaymentLink</returns>
        public PaymentLink CreatePaymentLink(List<RegistrationViewModel> regList, KiCData.Models.Event kicEvent, string[] discountCodes = null, string redirectUrl = null)
        {
            PaymentLink paymentLink = createPaymentLink(regList, kicEvent, discountCodes, redirectUrl);

            return paymentLink;
        }

        private PaymentLink createPaymentLink(List<RegistrationViewModel> regList, KiCData.Models.Event kicEvent, string[] discountCodes = null, string redirectUrl = null)
        {
            if(redirectUrl is null) redirectUrl = "https://www.kicevents.com/success";
            List<OrderLineItem> orderLineItems = new List<OrderLineItem>();
            List<OrderLineItemDiscount> orderDiscounts = new List<OrderLineItemDiscount>();

            var locations = _client.LocationsApi.ListLocations();
            string locationID = locations.Locations.FirstOrDefault().Id;

            foreach(RegistrationViewModel reg in regList)
            {
                ListCatalogResponse catalogResponse = _client.CatalogApi.ListCatalog();
                CatalogObject? catalogObject = catalogResponse.Objects
                    .Where(o => o.ItemData.Name == reg.Event.Name)
                    .FirstOrDefault();

                if(catalogObject is null)
                {
                    _logger.LogText("Could not find Catalog Object " + reg.Event.Name);
                    throw new Exception("Catalog object not found.");
                }

                string id = catalogObject.Id;

                CatalogObject? variation = catalogObject.ItemData.Variations.Where(v => v.ItemVariationData.Name == reg.TicketType).FirstOrDefault();

                if(variation is null)
                {
                    _logger.LogText("Could not find Item Variation " + reg.TicketType);
                    throw new Exception("Item variation not found.");
                }

                string varId = variation.Id;

                OrderLineItem orderLineItem = new OrderLineItem.Builder(quantity: "1")
                    .CatalogObjectId(varId)
                    .Note(reg.FirstName + " " + reg.LastName)
                    .Build();

                orderLineItems.Add(orderLineItem);
            }

            OrderServiceCharge orderServiceCharge = new OrderServiceCharge.Builder()
                .Name("Handling Fee")
                .Percentage("3")
                .CalculationPhase("SUBTOTAL_PHASE")
                .Build();

            List<OrderServiceCharge> serviceCharges = new List<OrderServiceCharge>();
            serviceCharges.Add(orderServiceCharge);

            OrderPricingOptions pricingOptions = new OrderPricingOptions.Builder()
                .AutoApplyTaxes(true)
                .Build();

            Order order = new Order.Builder(locationId: locationID)
                .LineItems(orderLineItems)
                .PricingOptions(pricingOptions)
                .ServiceCharges(serviceCharges)
                .Discounts(orderDiscounts)
                .Build();

            CheckoutOptions options = new CheckoutOptions.Builder()
                .RedirectUrl(redirectUrl)
                .Build();

            CreatePaymentLinkRequest paymentRequest = new CreatePaymentLinkRequest.Builder()
                .IdempotencyKey(Guid.NewGuid().ToString())
                .Order(order)
                .CheckoutOptions(options)
                .Build();

            PaymentLink paymentLink;

            try
            {
                CreatePaymentLinkResponse response = _client.CheckoutApi.CreatePaymentLink(paymentRequest);

                paymentLink = response.PaymentLink;
            }
            catch (Square.Exceptions.ApiException ex)
            {
                _logger.LogSquareEx(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw ex;
            }

            return paymentLink;
        }
    }
}

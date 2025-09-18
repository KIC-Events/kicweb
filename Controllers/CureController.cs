using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using KiCData.Services;
using KiCData.Models.WebModels;
using KiCData.Models.WebModels.PaymentModels;
using KiCData.Models.WebModels.PurchaseModels;
using KiCData.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using KiCWeb.Configuration;
using KiCWeb.Models;
using Event = KiCData.Models.Event;
using KiCData.Exceptions;
using KiCWeb.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Diagnostics;

namespace KiCWeb.Controllers
{
    [Route("cure")]
    public class CureController : KICController
    {
        private readonly KiCdbContext _kdbContext;
        private readonly FeatureFlags _featureFlags;
        private readonly ILogger _logger;
        private readonly PaymentService _paymentService;
        private readonly RegistrationSessionService _registrationSessionService;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IHttpContextAccessor _contextAccessor;

        public CureController(
            IConfigurationRoot configurationRoot,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            KiCdbContext kiCdbContext,
            ICookieService cookieService,
            PaymentService paymentService,
            ILogger<CureController> kiCLogger,
            RegistrationSessionService registrationSessionService,
            FeatureFlags featureFlags
        ) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
            _kdbContext = kiCdbContext ?? throw new ArgumentNullException(nameof(kiCdbContext));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _logger = kiCLogger ?? throw new ArgumentNullException(nameof(kiCLogger));
            _registrationSessionService = registrationSessionService ?? throw new ArgumentNullException(nameof(registrationSessionService));
            _configurationRoot = configurationRoot ?? throw new ArgumentNullException(nameof(configurationRoot));
            _featureFlags = featureFlags ?? throw new ArgumentNullException(nameof(featureFlags));
            _contextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        [Route("")]
        public IActionResult Index()
        {
            try
            {
                ViewBag.Sponsors = _kdbContext.Sponsors.ToList();
            }
            catch (Exception e)
            {
                ViewBag.Sponsors = new List<Sponsor>();
                _logger.LogError("Error loading sponsors. Setting sponsors to an empty list.");
                _logger.LogError(e.Message);
            }

            return View(); // Views/Cure/Index.cshtml
        }

        /// <summary>
        /// Displays the main registration page for the CURE event.
        /// Checks if registration is enabled via feature flags.
        /// Returns a 404 Not Found result if registration is disabled.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the Registration view, or NotFound if registration is disabled.
        /// </returns>
        [Route("registration")]
        public IActionResult Registration()
        {
            if (!_featureFlags.ShowCureRegistration)
            {
                return NotFound();
            }
            ViewBag.ShowCureRegForm = _featureFlags.ShowCureRegForm;
            return View(); // Views/Cure/Registration.cshtml
        }

        /// <summary>
        /// Displays the CURE event registration form.
        /// If a registration ID is provided, loads the existing registration for editing.
        /// Otherwise, prepares a new registration form with ticket and addon inventory.
        /// Returns a 404 Not Found result if the registration form feature flag is disabled.
        /// </summary>
        /// <param name="regId">The optional registration ID for editing an existing registration.</param>
        /// <returns>
        /// An <see cref="Task{IActionResult}"/> that renders the registration form view,
        /// or returns NotFound if the registration form is disabled.
        /// </returns>
        [HttpGet]
        [Route("registration/form")]
        public async Task<IActionResult> RegistrationForm(Guid? regId)
        {
            List<ItemInventory> ticketInventory;
            List<ItemInventory> addonInventory;
            try
            {
                ticketInventory = await _paymentService.GetItemInventoryAsync("CURE 2026");
                addonInventory = await _paymentService.GetItemInventoryAsync("Decadent Delight");
                ViewBag.TicketInventory = ticketInventory;
                ViewBag.Addon = addonInventory.First();
            }
            catch (InventoryException e)
            {
                _logger.LogError("An error occured while loading inventory items for the CURE registration form.");
                _logger.LogError(e.Message);
                throw;
            }

            if (regId != null)
            {
                // Look for existing registration in session
                var existingRegistration = _registrationSessionService.Registrations
                    .FirstOrDefault(r => r.RegId == regId);
                existingRegistration.TicketTypes =
                [
                    new SelectListItem("Gold", "Gold"),
                    new SelectListItem("Silver", "Silver"),
                    new SelectListItem("Sweet", "Sweet"),
                    new SelectListItem("Standard", "Standard"),
                ];

                existingRegistration.RoomTypes =
                [
                    new SelectListItem("King", "King"),
                    new SelectListItem("Doubles", "Doubles"),
                    new SelectListItem("Staying with someone else", "Staying with someone else"),
                    new SelectListItem("Not Staying in Host Hotel", "Not Staying in Host Hotel"),
                ];

                List<ItemInventory> _addons = addonInventory;
                ViewBag.Addon = _addons.First();
                ViewBag.TicketInventory = ticketInventory;
                ViewBag.IsUpdating = true;
                
                if(existingRegistration.DiscountCode is not null)
                {
                    TicketComp? ticketComp = _kdbContext.TicketComp
                        .Where(tc => tc.DiscountCode == existingRegistration.DiscountCode)
                        .FirstOrDefault();
                        
                    if(ticketComp is null)
                    {
                        ViewBag.Error = "Invalid Discount Code. Please check the discount code and try again.";
                        ViewBag.IsUpdating = false;
                    }
                }

                return View(existingRegistration);
            }

            if (!_featureFlags.ShowCureRegForm)
            {
                return NotFound();
            }

            RegistrationViewModel registration = new RegistrationViewModel()
            {
                Event = _kdbContext.Events
                .Where(
                    e => e.Id == int.Parse(_configurationRoot["CUREID"])
                )
                .First()
            };

            registration.TicketTypes = new List<SelectListItem>();
            var ticketInventoryList = ticketInventory;
            foreach (ItemInventory ti in ticketInventoryList)
            {
                SelectListItem item = new SelectListItem(ti.Name, ti.Name);
                if (ti.QuantityAvailable <= 0)
                {                 
                    item.Disabled = true;
                    item.Text = ti.Name + " - SOLD OUT";
                }

                registration.TicketTypes.Add(item);
            }

            registration.RoomTypes =
            [
              new SelectListItem("King", "King"),
              new SelectListItem("Doubles", "Doubles"),
              new SelectListItem("Staying with someone else", "Staying with someone else"),
              new SelectListItem("Not Staying in Host Hotel", "Not Staying in Host Hotel"),
            ];

            List<ItemInventory> addons = addonInventory;
            ViewBag.Addon = addons.First();
            ViewBag.TicketInventory = ticketInventoryList;
            ViewBag.IsUpdating = false;

            return View(registration); // Views/Cure/RegistrationForm.cshtml
        }

        /// <summary>
        /// Handles submission of the CURE event registration form.
        /// Processes meal addon and discount code, updates or adds the registration to the session,
        /// and redirects to the appropriate next step based on the user's action.
        /// </summary>
        /// <param name="registrationData">The <see cref="RegistrationViewModel"/> containing registration details.</param>
        /// <param name="action">The action indicating whether to create more registrations or proceed to payment.</param>
        /// <returns>
        /// An <see cref="Task{IActionResult}"/> that redirects to the registration form or payment page,
        /// or returns the view with error information if validation fails.
        /// </returns>
        [HttpPost]
        [Route("registration/form")]
        public async Task<IActionResult> RegistrationForm(RegistrationViewModel registrationData, string action)
        {
            // if (!ModelState.IsValid)
            // {
            //     ViewBag.Error = "Missing Required Information";
            //     return View(registrationData);
            // }

            if (registrationData.HasMealAddon == true)
            {
                registrationData.MealAddon = await _paymentService.GetAddonItemAsync();
            }

            var registrations = _registrationSessionService.Registrations;

            if (registrationData.DiscountCode is not null)
            {
                TicketComp? comp = _kdbContext.TicketComp
                    .Where(c => c.DiscountCode == registrationData.DiscountCode
                    && c.Ticket.EventId == int.Parse(_configurationRoot["CUREID"]))
                    .FirstOrDefault();

                if (comp is null)
                {
                    registrationData.RegId = Guid.NewGuid();
                    registrations.Add(registrationData);
                    return RedirectToAction("RegistrationForm", new RouteValueDictionary(new{controller = "cure", action = "RegistrationForm", regId = registrationData.RegId}));
                }

                registrationData.TicketComp = comp;
            }

            if (registrationData.RegId != Guid.Empty)
            {
                var existingRegistration = registrations
                    .FirstOrDefault(r => r.RegId == registrationData.RegId);

                if (existingRegistration != null)
                {
                    // Update existing registration by adding it at the same index
                    int index = registrations.IndexOf(existingRegistration);
                    registrations[index] = registrationData;
                    _registrationSessionService.Registrations = registrations;
                    return RedirectToAction("RegistrationPayment");
                }
            }

            // Set registrationData.RegID
            registrationData.RegId = Guid.NewGuid();


            registrationData.Price = _paymentService.GetTicketPrice(registrationData.TicketType);

            registrations.Add(registrationData);
            _registrationSessionService.Registrations = registrations;

            if (action == "CreateMore")
            {
                return RedirectToAction("RegistrationForm");
            }
            else
            {
                return RedirectToAction("RegistrationPayment");
            }
        }

        /// <summary>
        /// Removes a registration ticket from the current session based on the provided registration ID.
        /// If the registration is found, it is removed from the session.
        /// Returns a 204 No Content response.
        /// </summary>
        /// <param name="regId">The unique identifier of the registration to remove.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing a 204 No Content response.
        /// </returns>
        [HttpDelete]
        [Route("registration/{regId}")]
        public IActionResult RemoveTicket(Guid regId)
        {
            var registrations = _registrationSessionService.Registrations;
            var registrationToRemove = registrations.FirstOrDefault(r => r.RegId == regId);
            if (registrationToRemove != null)
            {
                registrations.Remove(registrationToRemove);
                _registrationSessionService.Registrations = registrations;
            }
            return NoContent();
        }

        /// <summary>
        /// Displays the payment page for CURE event registrations.
        /// Prepares registration and ticket inventory data, calculates total price,
        /// and redirects to the appropriate action if payment is not required or registration is missing.
        /// </summary>
        /// <returns>
        /// An <see cref="Task{IActionResult}"/> that renders the RegistrationPaymentForm view,
        /// or redirects to another action if conditions are not met.
        /// </returns>
        [HttpGet]
        [Route("registration/payment")]
        public async Task<IActionResult> RegistrationPayment()
        {
            var ticketInventory = _paymentService.GetItemInventoryAsync("CURE 2026");

            if (!_featureFlags.ShowCureRegistration)
            {
                return NotFound();
            }
            if (_registrationSessionService.IsEmpty())
            {
                // If no registration data is found, redirect to the registration form
                //TODO: ADD additional error handling or user feedback
                return RedirectToAction("RegistrationForm");
            }

            List<RegistrationViewModel> registrations = _registrationSessionService.Registrations;
            var ticketInventoryList = await ticketInventory;

            // Loop through registrations, match TicketType to ticketInventory[].Name.
            // Set TicketId to the SquareId, and set the Price
            foreach (RegistrationViewModel r in registrations)
            {
                foreach (ItemInventory ti in ticketInventoryList)
                {
                    if (r.TicketType == ti.Name)
                    {
                        r.Price = ti.Price;
                        r.TicketId = ti.SquareId;
                    }
                }
            }

            //Check if checkout total is 0 - if user is not paying, we can skip checkout screen.
            //Reduce price by discount ammount
            double? priceCheck = 0;
            foreach (RegistrationViewModel r in registrations)
            {
                priceCheck += r.Price;

                if (r.TicketComp is not null)
                {
                    priceCheck -= r.TicketComp.CompAmount;
                    r.Price -= (double)r.TicketComp.CompAmount;
                }

                if (r.MealAddon is not null)
                {
                    priceCheck += r.MealAddon.Price;
                }
            }

            if (priceCheck == 0)
            {
                var attendees = _paymentService.HandleNonPaymentCURETicketOrder(registrations);
                var orderId = CureRegistrationHelpers.FinalizeTicketOrder(_paymentService, registrations, attendees);
                return RedirectToAction("NoPay");
            }

            CureCardFormModel cfm = new CureCardFormModel();
            cfm.Items = registrations;

            ViewBag.AppId = _configurationRoot["Square:AppID"];
            ViewBag.LocationId = _configurationRoot["Square:LocationId"];

            return View(cfm); // Views/Cure/RegistrationPaymentForm.cshtml
        }

        /// <summary>
        /// Processes the payment for CURE event registrations.
        /// Updates registration items with ticket and event details, attempts payment using the provided card token,
        /// handles payment status, logs exceptions, and redirects to the appropriate result view.
        /// </summary>
        /// <param name="cfmUpdated">The updated <see cref="CureCardFormModel"/> containing registration and payment data.</param>
        /// <returns>
        /// An <see cref="Task{IActionResult}"/> that redirects to the success, error, or processing view based on payment outcome.
        /// </returns>
        [HttpPost]
        [Route("registration/payment")]
        public async Task<IActionResult> RegistrationPayment(CureCardFormModel cfmUpdated)
        {
            var ticketInventory = _paymentService.GetItemInventoryAsync("CURE 2026");

            Event CureEvent = _kdbContext.Events
                .Where(e => e.Id == int.Parse(_configurationRoot["CUREID"]))
                .First();

            // Loop through cfmUpdated.Items (which are the registrations).
            // Match TicketType to ticketInventory[].Name.
            // Set TicketId to the SquareId, and set the Price
            var ticketInventoryList = await ticketInventory;
            foreach (RegistrationViewModel r in cfmUpdated.Items)
            {
                foreach (ItemInventory ti in ticketInventoryList)
                {
                    if (r.TicketType == ti.Name)
                    {
                        r.Price = ti.Price;
                        r.TicketId = ti.SquareId;
                        r.Event = CureEvent;
                    }
                }
            }

            if (cfmUpdated.CardToken is not null)
            {
                string paymentStatus;
                List<Attendee> attendees;
                try
                {
                    paymentStatus = _paymentService.CreateCUREPayment(cfmUpdated.CardToken, cfmUpdated.BillingContact, cfmUpdated.Items, out attendees);
                }
                catch (Exception ex)
                {

                    if (ex is Square.Exceptions.ApiException squareEx)
                    {
                        // Handle Square-specific exceptions
                        _logger.LogError("A Square API exception occured. HTTP return code {statusCode}. {errorCount} errors occured.", squareEx.ResponseCode, squareEx.Errors.Count);
                        foreach (var err in squareEx.Errors)
                        {
                            _logger.LogError("[{errorCode} {errorCategory}]: {errorMessage}. Associated Field: {errorField}", err.Code, err.Category, err.Detail, err.Field);
                        }
                    }
                    else
                    {
                        // Handle other exceptions
                        _logger.LogError("An error occured while processing payment.");
                        _logger.LogError(ex.ToString());
                    }

                    return RedirectToAction("error");
                }


                if (paymentStatus.ToLower() == "approved" || paymentStatus.ToLower() == "completed")
                {
                    List<RegistrationViewModel> registrationViewModels = _registrationSessionService.Registrations;
                    var orderId = CureRegistrationHelpers.FinalizeTicketOrder(_paymentService, registrationViewModels, attendees);
                    return RedirectToAction("cardsuccess");
                }
                else if (paymentStatus.ToLower() == "canceled" || paymentStatus.ToLower() == "failed")
                {
                    return RedirectToAction("carderror");
                }
                else
                {
                    return RedirectToAction("paymentprocessing");
                }
            }


            return RedirectToAction("error");
        }

        [Route("paymentprocessing")]
        public IActionResult PaymentProcessing(string paymentId)
        {
            string paymentStatus = "pending";

            while (paymentStatus == "pending")
            {
                paymentStatus = _paymentService.CheckPaymentStatus(paymentId);
            }

            if (paymentStatus == "approved" || paymentStatus == "completed")
            {
                return RedirectToAction("cardsuccess");
            }
            else
            {
                return RedirectToAction("carderror");
            }

            throw new UnreachableException("No card status or unexpected card status");
        }

        [Route("carderror")]
        public IActionResult CardError()
        {
            // This action could be used to handle card errors
            // You might want to return a specific error view

            return View("CardError"); // Views/Cure/CardError.cshtml
        }

        /// <summary>
        /// Handles post-payment success logic for CURE event registrations.
        /// Marks attendees as paid, reduces ticket and addon inventory, retrieves the order ID,
        /// clears the registration session, and returns the CardSuccess view.
        /// </summary>
        /// <returns>
        /// An <see cref="Task{IActionResult}"/> that renders the CardSuccess view.
        /// </returns>
        [Route("cardsuccess")]
        public async Task<IActionResult> CardSuccess()
        {
            List<RegistrationViewModel> registrationViewModels = _registrationSessionService.Registrations;
            ViewBag.OrderID = _paymentService.getOrderID(registrationViewModels);
            _registrationSessionService.Clear();

            return View("CardSuccess"); // Views/Cure/CardSuccess.cshtml
        }

        [Route("rules")]
        public IActionResult Rules()
        {
            ViewBag.ShowHotelInfo = _featureFlags.ShowHotelInfo;
            return View(); // Views/Cure/Rules.cshtml
        }

        /// <summary>
        /// Displays the presenters page for the CURE event.
        /// Loads presenters, vendors, and presentations from the database,
        /// prepares presentation data for accordion display, and passes them to the view.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the Presenters view.
        /// </returns>
        [Route("presenters")]
        public IActionResult Presenters()
        {
            var cureEventId = _configurationRoot["CUREID"];
            ViewBag.Presenters = _kdbContext.Presenters
                .Include(p => p.Socials)
                .Include(p => p.Presentations)
                .OrderBy(p => p.PublicName)
                .ToList();

            ViewBag.Vendors = _kdbContext.Vendors
                .OrderBy(v => v.PublicName)
                .ToList();

            List<Presentation> presentations = [];
            if (cureEventId is not null)
            {
                presentations = _kdbContext.Presentations
                    .Include(p => p.Presenters)
                    .Where(p => p.EventId == int.Parse(_configurationRoot["CUREID"]))
                    .ToList();
            }

            ViewBag.Presentations = new List<AccordionItem>();

            foreach (Presentation p in presentations)
            {
                string concatName = p.Name;
                foreach (Presenter presenter in p.Presenters)
                {
                    concatName = concatName + " - " + presenter.PublicName;
                }

                AccordionItem accordionItem = new AccordionItem()
                {
                    Title = concatName,
                    Description = p.Description
                };

                ViewBag.Presentations.Add(accordionItem);
            }

            // Log to console or logger
            string json = JsonSerializer.Serialize(ViewBag.Vendors, new JsonSerializerOptions
            {
                WriteIndented = true // optional: makes it pretty
            });

            return View(); // Views/Cure/Presenters.cshtml
        }

        /// <summary>
        /// Displays the volunteers page for the CURE event.
        /// Returns a 404 Not Found result if the volunteers feature flag is disabled.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the Volunteers view, or NotFound if disabled.
        /// </returns>
        [Route("volunteers")]
        public IActionResult Volunteers()
        {
            if (!_featureFlags.ShowCureVolunteers)
            {
                return NotFound();
            }
            return View(); // Views/Cure/Volunteers.cshtml
        }

        [Route("error")]
        public IActionResult Error()
        {
            // This action could be used to handle errors
            // You might want to return a specific error view

            return View("Error"); // Views/Cure/Error.cshtml
        }

        [Route("success")]
        public IActionResult Success()
        {
            // This action could be used to show a success message after a successful operation
            // You might want to return a specific success view

            return View("Success"); // Views/Cure/Success.cshtml
        }

        /// <summary>
        /// Handles registrations that do not require payment.
        /// Marks attendees as paid, reduces ticket inventory, clears the registration session,
        /// and returns the NoPay view.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the NoPay view.
        /// </returns>
        [Route("nopay")]
        public async Task<IActionResult> NoPay()
        {
            List<RegistrationViewModel> registrationViewModels = _registrationSessionService.Registrations;
            ViewBag.OrderID = _paymentService.getOrderID(registrationViewModels);
            _registrationSessionService.Clear();

            return View();
        }
    }
}
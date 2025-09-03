using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using KiCData;
using KiCData.Services;
using KiCData.Models.WebModels;
using KiCData.Models.WebModels.PaymentModels;
using KiCData.Models.WebModels.PurchaseModels;
using KiCData.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Square;
using Square.Models;
using Square.Authentication;
using Square.Exceptions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using KiCWeb.Configuration;
using KiCWeb.Models;
using Event = KiCData.Models.Event;
using System.Threading.Tasks;

namespace KiCWeb.Controllers
{
    [Route("cure")]
    public class CureController : KICController
    {
        private readonly KiCdbContext _kdbContext;
        private readonly FeatureFlags _featureFlags;
        private readonly IPaymentService _paymentService;
        private readonly IKiCLogger _logger;
        private readonly RegistrationSessionService _registrationSessionService;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ItemSessionService _itemSessionService;

        public CureController(
            IConfigurationRoot configurationRoot,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            KiCdbContext kiCdbContext,
            ICookieService cookieService,
            IPaymentService paymentService,
            IKiCLogger kiCLogger,
            RegistrationSessionService registrationSessionService,
            FeatureFlags featureFlags,
            ItemSessionService itemSessionService
        ) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
            _kdbContext = kiCdbContext ?? throw new ArgumentNullException(nameof(kiCdbContext));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _logger = kiCLogger ?? throw new ArgumentNullException(nameof(kiCLogger));
            _registrationSessionService = registrationSessionService ?? throw new ArgumentNullException(nameof(registrationSessionService));
            _configurationRoot = configurationRoot ?? throw new ArgumentNullException(nameof(configurationRoot));
            _featureFlags = featureFlags ?? throw new ArgumentNullException(nameof(featureFlags));
            _contextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _itemSessionService = itemSessionService ?? throw new ArgumentNullException(nameof(itemSessionService));
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.Sponsors = _kdbContext.Sponsors.ToList();

            return View(); // Views/Cure/Index.cshtml
        }

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

        [HttpGet]
        [Route("registration/form")]
        public IActionResult RegistrationForm(Guid? regId)
        {
            var ticketInventory = _paymentService.GetTicketInventory("CURE 2026");
            ViewBag.TicketInventory = ticketInventory;

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

                ViewBag.IsUpdating = true;

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
            
            foreach(TicketInventory ti in ticketInventory)
            {
                SelectListItem item = new SelectListItem(ti.Name, ti.Name);
                if (ti.QuantityAvailable == 0)
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

            ViewBag.IsUpdating = false;

            return View(registration); // Views/Cure/RegistrationForm.cshtml
        }
        
        [HttpPost]
        [Route("registration/form")]
        public IActionResult RegistrationForm(RegistrationViewModel registrationData, string action)
        {
            // if (!ModelState.IsValid)
            // {
            //     ViewBag.Error = "Missing Required Information";
            //     return View(registrationData);
            // }
            
            if(registrationData.DiscountCode is not null)
            {
                TicketComp? comp = _kdbContext.TicketComp
                    .Where(c => c.DiscountCode == registrationData.DiscountCode)
                    .FirstOrDefault();
                    
                if(comp is null)
                {
                    ViewBag.Error = "Discount Code Invalid";
                    return View(registrationData);
                }

                registrationData.TicketComp = comp;             
            }
            
            var registrations = _registrationSessionService.Registrations;

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

        [HttpGet]
        [Route("registration/payment")]
        public IActionResult RegistrationPayment()
        {
            List<TicketInventory> ticketInventory = _paymentService.GetTicketInventory("CURE 2026");

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

            // Loop through registrations, match TicketType to ticketInventory[].Name.
            // Set TicketId to the SquareId, and set the Price
            foreach(RegistrationViewModel r in registrations)
            {
                foreach(TicketInventory ti in ticketInventory)
                {
                    if(r.TicketType == ti.Name)
                    {
                        r.Price = ti.Price;
                        r.TicketId = ti.SquareId;
                    }
                }
            }

            //Check if checkout total is 0 - if user is not paying, we can skip checkout screen.
            double? priceCheck = 0;
            foreach(RegistrationViewModel r in registrations)
            {
                priceCheck += r.Price;
                
                if(r.TicketComp is not null)
                {
                    priceCheck -= r.TicketComp.CompAmount;
                }
            }

            if (priceCheck == 0) RedirectToAction("nopay");

            CureCardFormModel cfm = new CureCardFormModel();
            cfm.Items = registrations;
            
            ViewBag.AppId = _configurationRoot["Square:AppID"];
            ViewBag.LocationId = _configurationRoot["Square:LocationId"];
            
            return View(cfm); // Views/Cure/RegistrationPaymentForm.cshtml
        }
        
        [HttpPost]
        [Route("registration/payment")]
        public IActionResult RegistrationPayment(CureCardFormModel cfmUpdated)
        {
            List<TicketInventory> ticketInventory = _paymentService.GetTicketInventory("CURE 2026");

            Event CureEvent = _kdbContext.Events
                .Where(e => e.Id == int.Parse(_configurationRoot["CUREID"]))
                .First();
            
            // Loop through cfmUpdated.Items (which are the registrations).
            // Match TicketType to ticketInventory[].Name.
            // Set TicketId to the SquareId, and set the Price
            foreach (RegistrationViewModel r in cfmUpdated.Items)
            {
                foreach (TicketInventory ti in ticketInventory)
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
                try
                {
                    paymentStatus = _paymentService.CreateCUREPayment(cfmUpdated.CardToken, cfmUpdated.BillingContact, cfmUpdated.Items);
                }
                catch (Exception ex)
                {

                    if (ex is Square.Exceptions.ApiException squareEx)
                    {
                        // Handle Square-specific exceptions
                        _logger.LogSquareEx(squareEx);
                    }
                    else
                    {
                        // Handle other exceptions
                        _logger.Log(ex);
                    }

                    return RedirectToAction("error");
                }


                if (paymentStatus.ToLower() == "approved" || paymentStatus.ToLower() == "completed")
                {
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
            
            if(paymentStatus == "approved" || paymentStatus == "completed")
            {
                return RedirectToAction("cardsuccess");
            }
            else
            {
                return RedirectToAction("carderror");
            }
            
            return NoContent(); 
        }
        
        [Route("carderror")]
        public IActionResult CardError()
        {
            // This action could be used to handle card errors
            // You might want to return a specific error view
            
            return View("CardError"); // Views/Cure/CardError.cshtml
        }
        
        [Route("cardsuccess")]
        public async Task<IActionResult> CardSuccess()
        {
            List<RegistrationViewModel> registrationViewModels = _registrationSessionService.Registrations;
            List<TicketAddon> ticketAddons = _itemSessionService.TicketAddons;
            await _paymentService.SetAttendeesPaidAsync(registrationViewModels);
            await _paymentService.ReduceTicketInventoryAsync(registrationViewModels);
            await _paymentService.ReduceAddonInventoryAsync(ticketAddons);
            
            return View("CardSuccess"); // Views/Cure/CardSuccess.cshtml
        }

        [Route("rules")]
        public IActionResult Rules()
        {
            ViewBag.ShowHotelInfo = _featureFlags.ShowHotelInfo;
            return View(); // Views/Cure/Rules.cshtml
        }

        [Route("presenters")]
        public IActionResult Presenters()
        {
            ViewBag.Presenters = _kdbContext.Presenters
                .Include(p => p.Socials)
                .Include(p => p.Presentations)
                .OrderBy(p => p.PublicName)
                .ToList();

            ViewBag.Vendors = _kdbContext.Vendors
                .OrderBy(v => v.PublicName)
                .ToList();

            List<Presentation> presentations = _kdbContext.Presentations
                .Include(p => p.Presenters)
                .Where(p => p.EventId == int.Parse(_configurationRoot["CUREID"]))
                .ToList();

            ViewBag.Presentations = new List<AccordionItem>();
            
            foreach(Presentation p in presentations)
            {
                string concatName = p.Name;
                foreach(Presenter presenter in p.Presenters)
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
        
        [Route("nopay")]
        public IActionResult NoPay()
        {
            return View();
        }
    }
}
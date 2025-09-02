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

        public CureController(
            IConfigurationRoot configurationRoot,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            KiCdbContext kiCdbContext,
            ICookieService cookieService,
            IPaymentService paymentService,
            IKiCLogger kiCLogger,
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
            if (!_featureFlags.ShowCureRegForm)
            {
                return NotFound();
            }

            var registrations = _registrationSessionService.Registrations;

            // Try to find an existing registration by regId
            RegistrationViewModel registration = null;
            if (regId.HasValue)
            {
                registration = registrations.FirstOrDefault(r => r.RegId == regId.Value);
            }

            // If not found, create a new registration
            if (registration == null)
            {
                registration = new RegistrationViewModel
                {
                    RegId = Guid.NewGuid(),
                    Event = _kdbContext.Events
                        .Where(e => e.Name == "CURE" && e.EndDate >= DateOnly.FromDateTime(DateTime.Now))
                        .FirstOrDefault()
                };

                registrations.Add(registration);
                _registrationSessionService.Registrations = registrations;
            }

            // Populate dropdowns
            registration.TicketTypes = new List<SelectListItem>
            {
                new SelectListItem("Gold", "Gold"),
                new SelectListItem("Silver", "Silver"),
                new SelectListItem("Sweet", "Sweet"),
                new SelectListItem("Standard", "Standard"),
            };

            registration.RoomTypes = new List<SelectListItem>
            {
                new SelectListItem("King", "King"),
                new SelectListItem("Doubles", "Doubles"),
                new SelectListItem("Staying with someone else", "Staying with someone else"),
                new SelectListItem("Not Staying in Host Hotel", "Not Staying in Host Hotel"),
            };

            return View(registration); // Views/Cure/RegistrationForm.cshtml
        }
        
        [HttpPost]
        [Route("registration/form")]
        public IActionResult RegistrationForm(RegistrationViewModel registrationData, string action)
        {
            // Log registrationData as json to console
            Console.WriteLine("RegistrationViewModel received:");
            Console.WriteLine(JsonSerializer.Serialize(registrationData, new JsonSerializerOptions { WriteIndented = true }));

            var registrations = _registrationSessionService.Registrations;
            registrationData.Price = 100; // TODO ⚠️⚠️⚠️ VERY IMPORTANT. MAKE THIS USE REAL TICKET PRICES ⚠️⚠️⚠️ VERY IMPORTANT. MAKE THIS USE REAL TICKET PRICES ⚠️⚠️⚠️
            registrations.Add(registrationData);
            Console.WriteLine(JsonSerializer.Serialize(registrationData, new JsonSerializerOptions { WriteIndented = true }));
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
        
        [HttpGet]
        [Route("registration/payment")]
        public IActionResult RegistrationPayment()
        {
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

            CureCardFormModel cfm = new CureCardFormModel();
            cfm.Items = _registrationSessionService.Registrations;
            //delete registrationStorage.Registrations; // Clear the registrations after payment form is created
            
            ViewBag.AppId = _configurationRoot["Square:AppID"];
            ViewBag.LocationId = _configurationRoot["Square:LocationId"];
            
            return View(cfm); // Views/Cure/RegistrationPaymentForm.cshtml
        }
        
        [HttpPost]
        [Route("registration/payment")]
        public IActionResult RegistrationPayment(CureCardFormModel cfmUpdated)
        {
            Console.WriteLine("CureCardFormModel received:");
            Console.WriteLine(JsonSerializer.Serialize(cfmUpdated, new JsonSerializerOptions { WriteIndented = true }));
            
            if (cfmUpdated.CardToken is not null)
            {
                string paymentStatus;
                Console.WriteLine("Here 1");
                try
                {
                    paymentStatus = _paymentService.CreateCUREPayment(cfmUpdated.CardToken, cfmUpdated.BillingContact, cfmUpdated.Items);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception during payment processing:");
                    Console.WriteLine(ex.ToString());

                Console.WriteLine("Here 2");

                    if (ex is Square.Exceptions.ApiException squareEx)
                    {
                     Console.WriteLine("Square Payment failed:");
                                        Console.WriteLine($"Status Code: {squareEx.ResponseCode}");
                    Console.WriteLine($"Message: {squareEx.Message}");
                    Console.WriteLine($"RawBody: {squareEx.HttpContext?.Response?.RawBody}");
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

                Console.WriteLine("Here 3");


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

                Console.WriteLine("Here 4");

            
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
            
            return View(); // Views/Cure/PaymentProcessing.cshtml
        }
        
        [Route("carderror")]
        public IActionResult CardError()
        {
            // This action could be used to handle card errors
            // You might want to return a specific error view
            
            return View("CardError"); // Views/Cure/CardError.cshtml
        }
        
        [Route("cardsuccess")]
        public IActionResult CardSuccess()
        {
            // This action could be used to show a success message after a successful card operation
            // You might want to return a specific success view
            
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
            Console.WriteLine("Volunteers page accessed.");
            Console.WriteLine($"Feature flag for ShowCureVolunteers: {_featureFlags.ShowCureVolunteers}");
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
    }
}
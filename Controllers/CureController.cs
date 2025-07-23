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

namespace KiCWeb.Controllers
{
    [Route("cure")]
    public class CureController : KICController
    {
        private readonly KiCdbContext _kdbContext;
        private readonly IPaymentService _paymentService;
        private readonly IKiCLogger _logger;
        private readonly RegistrationStorageService _registrationStorageService;
        private readonly IConfigurationRoot _configurationRoot;

        public CureController(
            IConfigurationRoot configurationRoot,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            KiCdbContext kiCdbContext,
            ICookieService cookieService,
            IPaymentService paymentService,
            IKiCLogger kiCLogger,
            RegistrationStorageService registrationStorageService
        ) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
            _kdbContext = kiCdbContext ?? throw new ArgumentNullException(nameof(kiCdbContext));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _logger = kiCLogger ?? throw new ArgumentNullException(nameof(kiCLogger));
            _registrationStorageService = registrationStorageService ?? throw new ArgumentNullException(nameof(registrationStorageService));
            _configurationRoot = configurationRoot ?? throw new ArgumentNullException(nameof(configurationRoot));
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
            return View(); // Views/Cure/Registration.cshtml
        }

        [HttpGet]
        [Route("registration/form")]
        public IActionResult RegistrationForm()
        {
            // This action could be used to return a form for registration
            // You might want to return a partial view or a specific view for the form

            RegistrationViewModel registration = new RegistrationViewModel()
            {
                Event = _kdbContext.Events
                .Where(
                    e => e.Name == "CURE"
                    && e.EndDate >= DateOnly.FromDateTime(DateTime.Now) // Ensure the event is not in the past, and that it is the CURE event
                )
                .First()
            };

            registration.TicketTypes =
            [
              new SelectListItem("Gold", "Gold"),
              new SelectListItem("Silver", "Silver"),
              new SelectListItem("Sweet", "Sweet"),
              new SelectListItem("Standard", "Standard"),
            ];

            registration.RoomTypes =
            [
              new SelectListItem("King", "King"),
              new SelectListItem("Doubles", "Doubles"),
              new SelectListItem("Staying with someone else", "Staying with someone else"),
              new SelectListItem("Not Staying in Host Hotel", "Not Staying in Host Hotel"),
            ];

            registration.ArrivalDays =
            [
              new SelectListItem("Thursday evening", "Thursday evening"),
              new SelectListItem("Friday morning", "Friday morning"),
              new SelectListItem("Friday afternoon", "Friday afternoon"),
              new SelectListItem("Other", "Other"),
            ];
            
            return View(registration); // Views/Cure/RegistrationForm.cshtml
        }
        
        [HttpPost]
        [Route("registration/form")]
        public IActionResult RegistrationForm(RegistrationViewModel registrationData)
        {
            string sessionId = HttpContext.Session.Id;

            RegistrationStorage? registrationStorage = _registrationStorageService.Storage
                .Where(s => s.SessionID == sessionId)
                .FirstOrDefault();
                
            if (registrationStorage is null)
            {
                registrationStorage = new RegistrationStorage
                {
                    SessionID = sessionId,
                    Registrations = new List<RegistrationViewModel>()
                };
                _registrationStorageService.Storage.Add(registrationStorage);
            }
            registrationStorage.Registrations.Add(registrationData);
            
            if(registrationData.CreateMore)
            {
                return RedirectToAction("RegistrationForm");
            }
            else
            {
                // Redirect to the payment page with the registration data
                return RedirectToAction("RegistrationPayment");
            }
        }
        
        [HttpGet]
        [Route("registration/payment")]
        public IActionResult RegistrationPayment()
        {
            string sessionId = HttpContext.Session.Id;

            RegistrationStorage? registrationStorage = _registrationStorageService.Storage
                .Where(s => s.SessionID == sessionId)
                .FirstOrDefault();
                
            if (registrationStorage is null || !registrationStorage.Registrations.Any())
            {
                // If no registration data is found, redirect to the registration form
                //TODO: ADD additional error handling or user feedback
                return RedirectToAction("RegistrationForm");
            }
            
            CureCardFormModel cfm = new CureCardFormModel();
            cfm.Items = registrationStorage.Registrations;
            //delete registrationStorage.Registrations; // Clear the registrations after payment form is created
            
            ViewBag.AppId = _configurationRoot["Square:AppID"];
            ViewBag.LocationId = _configurationRoot["Square:LocationId"];
            
            return View(cfm); // Views/Cure/RegistrationPaymentForm.cshtml
        }
        
        [HttpPost]
        [Route("registration/payment")]
        public IActionResult RegistrationPayment(CureCardFormModel cfmUpdated)
        {
            if(cfmUpdated.CardToken is not null)
            {
                string paymentStatus;
                try
                {
                    paymentStatus = _paymentService.CreateCUREPayment(cfmUpdated.CardToken, cfmUpdated.BillingContact, cfmUpdated.Items);
                }
                catch(Exception ex)
                {
                    if(ex is Square.Exceptions.ApiException squareEx)
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
                
                if(paymentStatus.ToLower() == "approved" || paymentStatus.ToLower() == "completed")
                {
                    return RedirectToAction("cardsuccess");
                }
                else if(paymentStatus.ToLower() == "canceled" || paymentStatus.ToLower() == "failed")
                {
                    return RedirectToAction("carderror")
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

            // Log to console or logger
            string json = JsonSerializer.Serialize(ViewBag.Vendors, new JsonSerializerOptions
            {
                WriteIndented = true // optional: makes it pretty
            });
            Console.WriteLine("Vendors JSON:");
            Console.WriteLine(json); // or use your logger
            
            return View(); // Views/Cure/Presenters.cshtml
        }

        [Route("volunteers")]
        public IActionResult Volunteers()
        {
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
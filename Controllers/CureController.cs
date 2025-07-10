using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using KiCData;
using KiCData.Services;
using KiCData.Models.WebModels;
using KiCData.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Square;
using Square.Models;
using Square.Authentication;
using Square.Exceptions;

namespace KiCWeb.Controllers
{
    [Route("cure")]
    public class CureController : KICController
    {
        private readonly KiCdbContext _kdbContext;
        private readonly InternalPaymentService _paymentService;
        private readonly IKiCLogger _logger;

        public CureController(
            IConfigurationRoot configurationRoot,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            KiCdbContext kiCdbContext,
            ICookieService cookieService,
            InternalPaymentService paymentService,
            IKiCLogger kiCLogger
        ) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
            _kdbContext = kiCdbContext ?? throw new ArgumentNullException(nameof(kiCdbContext));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _logger = kiCLogger ?? throw new ArgumentNullException(nameof(kiCLogger));
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
            
            CardFormModel cfm = new CardFormModel();
            
            return View(cfm); // Views/Cure/RegistrationForm.cshtml
        }
        
        [HttpPost]
        [Route("registration/form")]
        public IActionResult RegistrationForm(CardFormModel cfmUpdated)
        {
            if(cfmUpdated.CardToken is not null)
            {
                try
                {
                    _paymentService.CreateGenericPayment(cfmUpdated.CardToken, cfmUpdated.BillingContact, cfmUpdated.Items);
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
                
                return RedirectToAction("paymentprocessing");
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
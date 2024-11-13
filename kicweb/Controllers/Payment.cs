using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Square.Models;

namespace KiCWeb.Controllers
{
    [Route("[controller]")]
    public class Payment : KICController
    {
        private readonly ILogger<Payment> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICookieService _cookieService;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly KiCdbContext _context;
        private readonly IPaymentService _paymentService;

        public Payment(ILogger<Payment> logger, IHttpContextAccessor contextAccessor, ICookieService cookieService, IConfigurationRoot configurationRoot, KiCdbContext kiCdbContext, IPaymentService paymentService) : base(configurationRoot, userService: null, contextAccessor, kiCdbContext, cookieService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _cookieService = cookieService;
            _configurationRoot = configurationRoot;
            _context = kiCdbContext;
            _paymentService = paymentService;
        }

        [HttpGet("Purchase")]
        public IActionResult Purchase()
        {
            return View();
        }

        [HttpGet("Merch")]
        [Route("/Merch")]
        public IActionResult MerchStore()
        {
            return Redirect("https://kic-events.square.site/shop/apparel/INJSIHWIBYY7LG4HENI4NYFL");
        }

        [HttpGet("Blasphemy")]
        [Route("/Blasphemy")]
        [Route("~/Blasphemy")]
        public IActionResult Blasphemy()
        {
            ViewBag.SalesActive = true;

            if (DateTime.Now >= new DateTime(2024, 12, 15, 10, 0, 0))
            {
                ViewBag.SalesActive = false;
            }

            int standardCnt = _paymentService.CheckInventory("Blasphemy Ticket", "Standard");
            int vipCnt = _paymentService.CheckInventory("Blasphemy Ticket", "VIP");

            ViewBag.StandardCount = standardCnt;
            ViewBag.VIPCount = vipCnt;

            RegistrationViewModel viewModel = new RegistrationViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Blasphemy(RegistrationViewModel rvmUpdated)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Missing or incorrect registration info.";
                return View(rvmUpdated);
            }

            if(rvmUpdated.Email != rvmUpdated.EmailConf)
            {
                ViewBag.ErrorMessage = "Email does not match.";
                return View(rvmUpdated);
            }

            AddRegToCookies(rvmUpdated);

            if(rvmUpdated.CreateMore == false)
            {
                return Redirect("InterstitialWait");
            }
            else
            {
                return Redirect("~/Blasphemy");
            }
        }

        [Route("~/InterstitialWait")]
        [Route("/Home/InterstitialWait")]
        [Route("/GenController/InterstitialWait")]
        public IActionResult InterstitialWait()
        {
            return View();
        }

        [Route("~/PaymentProcess")]
        [Route("/Home/PaymentProcess")]
        [Route("/Payment/PaymentProcess")]
        public IActionResult PaymentProcess(string eventName, int price)
        {
            List<RegistrationViewModel> regList = GetRegFromCookies();
            _cookieService.DeleteCookie(_contextAccessor.HttpContext.Request, "Registration");
            PaymentLink paymentURL;
            try
            {
                paymentURL = _paymentService.CreateCurePaymentLink(regList);
            }
            catch (Exception ex)
            {
                return Redirect("Error");
            }

            WriteRegToDB(regList, paymentURL.OrderId, paymentURL.Id, eventName, price);

            return Redirect(paymentURL.Url);
        }

        private void AddRegToCookies(RegistrationViewModel rvm)
        {
            var context = _contextAccessor.HttpContext;

            if (context.Request.Cookies["Registration"] is null)
            {
                CookieOptions cookieOptions = _cookieService.NewCookieFactory();
                List<RegistrationViewModel> regList = new List<RegistrationViewModel>();
                regList.Add(rvm);
                string cookieValue = JsonConvert.SerializeObject(regList);
                context.Response.Cookies.Append("Registration", cookieValue, cookieOptions);
            }
            else
            {
                List<RegistrationViewModel> regList = JsonConvert.DeserializeObject<List<RegistrationViewModel>>(context.Request.Cookies["Registration"]);
                regList.Add(rvm);
                context.Response.Cookies.Delete("Registration");
                string cookieValue = JsonConvert.SerializeObject(regList);
                CookieOptions cookieOptions = _cookieService.NewCookieFactory();
                context.Response.Cookies.Append("Registration", cookieValue, cookieOptions);
            }
        }

        private List<RegistrationViewModel> GetRegFromCookies()
        {
            var context = _contextAccessor.HttpContext;

            string? regList = context.Request.Cookies["Registration"];

            if (regList == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                List<RegistrationViewModel>? convertedRegList = JsonConvert.DeserializeObject<List<RegistrationViewModel>>(regList);
                return convertedRegList;
            }
        }

        private void WriteRegToDB(List<RegistrationViewModel> regList, string orderID, string paymentLinkID, string eventName, int price)
        {
            KiCData.Models.Event? kicEvent = _context.Events.Where(e => e.Name == eventName).FirstOrDefault();

            foreach (var reg in regList)
            {
                KiCData.Models.Member? member = _context.Members.Where(m => m.FirstName == reg.FirstName && m.LastName == reg.LastName).FirstOrDefault();

                if(member == null)
                {
                    member = new()
                    {
                        FirstName = reg.FirstName,
                        LastName = reg.LastName,
                        Email = reg.Email,
                        DateOfBirth = reg.DateOfBirth
                    };
                }

                Ticket ticket = new() 
                {
                    Event = kicEvent,
                    Price = price,
                    DatePurchased = DateOnly.FromDateTime(DateTime.Today),
                    StartDate = kicEvent.StartDate,
                    EndDate = kicEvent.EndDate,
                    Type = reg.TicketType
                };

                Attendee attendee = new()
                {
                    Member = member,
                    OrderID = orderID,
                    PaymentLinkID = paymentLinkID,
                    Ticket = ticket
                };

                ticket.Attendee = attendee;

                _context.Members.Add(member);
                _context.Ticket.Add(ticket);
                _context.Attendees.Add(attendee);
            }

            _context.SaveChanges();
        }


        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View("Error!");
        //}
    }
}
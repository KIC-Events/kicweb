using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
	    private KiCdbContext _kdbContext;

        public Payment(ILogger<Payment> logger, IHttpContextAccessor contextAccessor, ICookieService cookieService, IConfigurationRoot configurationRoot, KiCdbContext kiCdbContext, IPaymentService paymentService) : base(configurationRoot, userService: null, contextAccessor, kiCdbContext, cookieService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _cookieService = cookieService;
            _configurationRoot = configurationRoot;
            _context = kiCdbContext;
            _paymentService = paymentService;
            _kdbContext = kiCdbContext;
        }

        [HttpGet("Purchase")]
        public IActionResult Purchase()
        {
            _cookieService.DeleteCookie(_contextAccessor.HttpContext.Request, "Registration");

            List<KiCData.Models.Event> events = _kdbContext.Events
                        .Where(e => e.StartDate > DateOnly.FromDateTime(DateTime.Now))
                        .ToList();
                    
            List<KiCData.Models.Event> eventsWithImages = new List<KiCData.Models.Event>();
            foreach(KiCData.Models.Event e in events)
            {
                if(e.ImagePath is not null)
                {
                    eventsWithImages.Add(e);
                }
            }
            
            ViewBag.events = eventsWithImages; // New events for redesign (TODO refactor this)
                    
            return View();
        }

        [HttpGet("Merch")]
        [Route("/Merch")]
        public IActionResult MerchStore()
        {
            return Redirect("https://kic-events.square.site/shop/apparel/INJSIHWIBYY7LG4HENI4NYFL");
        }


        [Route("/Blasphemy")]
        [Route("~/Blasphemy")]
        [Route("/Payment/Blasphemy")]
        [HttpGet]
        public IActionResult Blasphemy()
        {
            ViewBag.SalesActive = true;

            if (DateTime.Now >= new DateTime(2024, 12, 15, 10, 0, 0))
            {
                ViewBag.SalesActive = false;
            }

            int standardCnt = _paymentService.CheckInventory("Blasphemy", "Standard");
            int vipCnt = _paymentService.CheckInventory("Blasphemy", "VIP");

            ViewBag.StandardCount = standardCnt;
            ViewBag.VIPCount = vipCnt;

            RegistrationViewModel viewModel = new RegistrationViewModel()
            {
                Event = _context.Events.Where(e => e.Name == "Blasphemy").First()
            };

            viewModel.TicketTypes = new List<SelectListItem>();
            if (vipCnt > 0) { viewModel.TicketTypes.Add(new SelectListItem("VIP", "VIP")); }
            viewModel.TicketTypes.Add(new SelectListItem("Standard", "Standard"));

            return View(viewModel);
        }


        [Route("/Blasphemy")]
        [Route("~/Blasphemy")]
        [Route("/Payment/Blasphemy")]
        [HttpPost]
        public IActionResult Blasphemy(RegistrationViewModel rvmUpdated)
        {
            if (rvmUpdated.TicketType == "Standard")
            {
                rvmUpdated.Price = 15;
            }
            else if(rvmUpdated.TicketType == "VIP")
            {
                rvmUpdated.Price = 30;
            }

            rvmUpdated.Event = _context.Events.Where(e => e.Name == "Blasphemy").First();

            AddRegToCookies(rvmUpdated);

            if(rvmUpdated.CreateMore == false)
            {
                return Redirect("/Payment/InterstitialWait");
            }
            else
            {
                return RedirectToAction("Blasphemy");
            }
        }

        [Route("~/InterstitialWait")]
        [Route("/Home/InterstitialWait")]
        [Route("/Payment/InterstitialWait")]
        public IActionResult InterstitialWait()
        {
            return View();
        }

        [Route("~/PaymentProcess")]
        [Route("/Home/PaymentProcess")]
        [Route("/Payment/PaymentProcess")]
        public IActionResult PaymentProcess()
        {
            List<RegistrationViewModel> regList = GetRegFromCookies();
            _cookieService.DeleteCookie(_contextAccessor.HttpContext.Request, "Registration");

            PaymentLink paymentURL;

            try
            {
                paymentURL = _paymentService.CreatePaymentLink(regList, regList[0].Event);
            }
            catch (Exception ex)
            {
                return Redirect("~/Error");
            }

            WriteRegToDB(regList, paymentURL.OrderId, paymentURL.Id);

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
                string cookieValue = JsonConvert.SerializeObject(regList, new JsonSerializerSettings(){
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                context.Response.Cookies.Append("Registration", cookieValue, cookieOptions);
            }
            else
            {
                List<RegistrationViewModel> regList = JsonConvert.DeserializeObject<List<RegistrationViewModel>>(context.Request.Cookies["Registration"], new JsonSerializerSettings(){
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                regList.Add(rvm);
                context.Response.Cookies.Delete("Registration");
                string cookieValue = JsonConvert.SerializeObject(regList, new JsonSerializerSettings(){
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
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

        private void WriteRegToDB(List<RegistrationViewModel> regList, string orderID, string paymentLinkID)
        {
            KiCData.Models.Event? kicEvent = _context.Events.Where(e => e.Name == regList[0].Event.Name).FirstOrDefault();

            foreach (var reg in regList)
            {
                KiCData.Models.Member? member = _context.Members.Where(m => m.FirstName == reg.FirstName && m.LastName == reg.LastName).FirstOrDefault();

                if(member == null)
                {
                    member = new()
                    {
                        FirstName = reg.FirstName,
                        LastName = reg.LastName,
                        Email = "N/A",
                        DateOfBirth = reg.DateOfBirth,
                        AdditionalInfo = "N/A",
                        City = "N/A",
                        State = "N/A"
                    };

                    _context.Members.Add(member);
                }

                Ticket ticket = new() 
                {
                    Event = kicEvent,
                    Price = (double)reg.Price,
                    DatePurchased = DateOnly.FromDateTime(DateTime.Today),
                    StartDate = kicEvent.StartDate,
                    EndDate = kicEvent.EndDate,
                    Type = reg.TicketType,
                    IsComped = false
                };

                Attendee attendee = new()
                {
                    Member = member,
                    OrderID = orderID,
                    PaymentLinkID = paymentLinkID,
                    Ticket = ticket,
                    BackgroundChecked = false,
                    BadgeName = "N/A",
                    ConfirmationNumber = 1215,
                    IsPaid = false,
                    IsRefunded = false,
                    isRegistered = true,
                    Pronouns = "N/A",
                    RoomPreference = "N/A",
                    RoomWaitListed = false,
                    TicketWaitListed = false
                };

                ticket.Attendee = attendee;

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
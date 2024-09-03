using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using cure.Models;
using Cure.Models;
using Square;
using KiCData.Models.WebModels.Member;
using Newtonsoft.Json;
using Square.Models;
using Org.BouncyCastle.Ocsp;


namespace cure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IConfigurationRoot _config;
        private readonly ICookieService _cookieService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KiCdbContext _kdbContext;

        public HomeController(ILogger<HomeController> logger, IPaymentService paymentService, IConfigurationRoot configuration, ICookieService cookieService, IHttpContextAccessor contextAccessor, KiCdbContext kiCdbContext)
        {
            _logger = logger;
            _paymentService = paymentService;
            _config = configuration;
            _cookieService = cookieService;
            _contextAccessor = contextAccessor;
            _kdbContext = kiCdbContext;
        }

        [Route("~/")]
        [Route("~/Home")]
        [Route("~/Home/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            BetaViewModel bvm = new BetaViewModel();
            return View(bvm);
        }

        [HttpPost]
        public IActionResult Index(BetaViewModel bvmUpdated)
        {
            if(bvmUpdated.BetaCode == _config["AppData:Beta_Code"])
            {
                ViewBag.Beta = true;
            }
            else
            {
                ViewBag.Beta = false;
            }

            return View(bvmUpdated);
        }

        public IActionResult ComingSoon()
        {
            return View();         
        }

        public IActionResult Presenters()
        {
            //return View();
            return RedirectToAction("ComingSoon");
        }

        public IActionResult Vendors()
        {
            //return View();
            return RedirectToAction("ComingSoon");
        }

        public IActionResult Rules()
        {
            //return View();
            return RedirectToAction("ComingSoon");
        }

        public IActionResult Schedule()
        {
            //return View();
            return RedirectToAction("ComingSoon");
        }

        [Route("~/Register")]
        [HttpGet]
        public IActionResult Registration()
        {
            int goldCount = 0;
            int silverCount = 0;
            int regularCount = 0;
            try
            {
                silverCount = _paymentService.CheckInventory("CURE Event Ticket", "Silver");
                goldCount = _paymentService.CheckInventory("CURE Event Ticket", "Gold");
                regularCount = _paymentService.CheckInventory("CURE Event Ticket", "Regular");
            }
            catch (Exception ex)
            {
                return Redirect("Error");
                //TODO Log exception
            }
            
            RegistrationViewModel reg = new RegistrationViewModel();

            reg.TicketTypes = new List<SelectListItem>();
            if (goldCount > 0) { reg.TicketTypes.Add(new SelectListItem("Gold - " + goldCount.ToString() + " Remaining", "Gold")); }
            if (silverCount > 0) { reg.TicketTypes.Add(new SelectListItem("Silver - " + silverCount.ToString() + " Remaining", "Silver")); }
            reg.TicketTypes.Add(new SelectListItem("Early Pricing - Regular - " + regularCount.ToString(), "Regular"));

            reg.RoomTypes = new List<SelectListItem>();
            reg.RoomTypes.Add(new SelectListItem("One King", "One King"));
            reg.RoomTypes.Add(new SelectListItem("Two Doubles", "Two Doubles"));
            reg.RoomTypes.Add(new SelectListItem("I will not be staying at the host hotel.", "I will not be staying at the host hotel."));

            if (goldCount == 0 && silverCount == 0 && regularCount == 0)
            {
                ViewBag.SoldOut = true;
                reg.WaitList = true;
            }

            return View(reg);
        }

        [Route("~/Register")]
        [HttpPost]
        public IActionResult Registration(RegistrationViewModel regUpdated)
        {
            if (!ModelState.IsValid)
            {


                ViewBag.Error = "Missing required info.";
                return View();
            }

            if(regUpdated.Email != regUpdated.EmailConf)
            {
                ViewBag.Error = "Email does not match.";
                return View();
            }

            if(regUpdated.DiscountCode is not null)
            {
                TicketComp? comp = _kdbContext.TicketComp
                    .Where(c => c.DiscountCode ==  regUpdated.DiscountCode)
                    .FirstOrDefault();

                if (comp != null)
                {
                    regUpdated.TicketComp = comp;
                }
                else
                {
                    ViewBag.Error = "Discount code not found.";
                    return View(regUpdated);
                }
            }

            AddRegToCookies(regUpdated);

            if(regUpdated.CreateMore == false)
            {
                return Redirect("Payment");
            }
            else
            {
                return Redirect("Register");
            }            
        }

        [Route("~/Payment")]
        public IActionResult Payment()
        {
            List<RegistrationViewModel> regList = GetRegFromCookies();
            _cookieService.DeleteCookie(_contextAccessor.HttpContext.Request, "Registration");
            WriteRegToDB(regList);
            string paymentURL = _paymentService.CreatePaymentLink(regList);
            return Redirect(paymentURL);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("~/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new cure.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("~/Success")]
        public IActionResult Success()
        {
            return View();
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

            if(regList == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                List<RegistrationViewModel>? convertedRegList = JsonConvert.DeserializeObject<List<RegistrationViewModel>>(regList);
                return convertedRegList;
            }
        }

        private void WriteRegToDB(List<RegistrationViewModel> regList)
        {
            //KiCData.Models.Event? CURE = _kdbContext.Events.Where(e => e.Id == 1112).FirstOrDefault();
            //if(CURE == null)
            //{
            //
            //}
            foreach(var reg in regList)
            {
                if(reg.TicketComp is null)
                {
                    Ticket ticket = new Ticket()
                    {
                        EventId = 1112,
                        //Event = CURE,
                        Type = reg.TicketType,
                        DatePurchased = DateOnly.FromDateTime(DateTime.Now),
                        //StartDate = CURE.StartDate,
                        //EndDate = CURE.EndDate,
                        IsComped = reg.TicketComp is not null ? true : false,
                        Attendee = new Attendee()
                        {
                            BadgeName = reg.BadgeName,
                            BackgroundChecked = false,
                            ConfirmationNumber = Guid.NewGuid().GetHashCode(),
                            RoomWaitListed = true,
                            TicketWaitListed = reg.WaitList,
                            RoomPreference = reg.RoomType,
                            IsPaid = false,
                            isRegistered = true,
                            Member = new Member()
                            {
                                FirstName = reg.FirstName,
                                LastName = reg.LastName,
                                Email = reg.Email,
                                DateOfBirth = reg.DateOfBirth,
                                FetName = reg.FetName,
                                ClubId = reg.ClubId,
                                PhoneNumber = reg.PhoneNumber
                            }
                        }
                    };

                    _kdbContext.Members.Add(ticket.Attendee.Member);
                    _kdbContext.Attendees.Add(ticket.Attendee);
                    _kdbContext.Ticket.Add(ticket);
                    _kdbContext.SaveChanges();
                }
                else
                {
                    Ticket ticket = _kdbContext.Ticket
                        .Where(t => t.Id == reg.TicketComp.TicketId)
                        .FirstOrDefault();

                    Attendee attendee = _kdbContext.Attendees
                        .Where(a => a.TicketId == reg.TicketComp.TicketId)
                        .FirstOrDefault();

                    Member member = _kdbContext.Members
                        .Where(m => m.Id == attendee.MemberId)
                        .FirstOrDefault();

                    ticket.Price = 160;
                    ticket.Type = reg.TicketType;
                    ticket.DatePurchased = DateOnly.FromDateTime(DateTime.Now);
                    //ticket.StartDate = CURE.StartDate;
                    //ticket.EndDate = CURE.EndDate;
                    ticket.IsComped = true;

                    attendee.MemberId = member.Id;
                    attendee.TicketId = ticket.Id;
                    attendee.BadgeName = reg.BadgeName;
                    attendee.BackgroundChecked = false;
                    attendee.ConfirmationNumber = Guid.NewGuid().GetHashCode();
                    attendee.RoomWaitListed = true;
                    attendee.TicketWaitListed = reg.WaitList;
                    attendee.RoomPreference = reg.RoomType;
                    attendee.IsPaid = false;
                    attendee.isRegistered = true;

                    member.FirstName = reg.FirstName;
                    member.LastName = reg.LastName;
                    member.Email = reg.Email;
                    member.DateOfBirth = reg.DateOfBirth;
                    member.FetName = reg.FetName;
                    member.ClubId = reg.ClubId;
                    member.PhoneNumber = reg.PhoneNumber;

                    _kdbContext.SaveChanges();
                }

                if(reg.WillVolunteer == true)
                {
                    PendingVolunteer pVol = new PendingVolunteer()
                    {
                        FirstName = reg.FirstName,
                        LastName = reg.LastName,
                        Email = reg.Email,
                        //Event = CURE,
                        EventId = 1112
                    };

                    _kdbContext.PendingVolunteers.Add(pVol);
                    _kdbContext.SaveChanges();
                }
            }
        }
    }
}

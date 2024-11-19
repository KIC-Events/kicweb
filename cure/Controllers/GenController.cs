using KiCData.Models.WebModels;
using KiCData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Square.Models;
using System.Diagnostics;
using cure.Controllers;
using KiCData.Services;

namespace Cure.Controllers
{
    public class GenController : KICController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IConfigurationRoot _config;
        private readonly ICookieService _cookieService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KiCdbContext _kdbContext;

        public GenController(ILogger<HomeController> logger, IPaymentService paymentService, IConfigurationRoot configuration, ICookieService cookieService, IHttpContextAccessor contextAccessor, KiCdbContext kiCdbContext, IUserService userService) : base(configuration, userService, contextAccessor, kiCdbContext, cookieService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _config = configuration;
            _cookieService = cookieService;
            _contextAccessor = contextAccessor;
            _kdbContext = kiCdbContext;
        }

        [Route("~/ComingSoon")]
        [Route("/Home/ComingSoon")]
        [Route("/GenController/ComingSoon")]
        public IActionResult ComingSoon()
        {
            return View();
        }

        [Route("~/Presenters")]
        [Route("/Home/Presenters")]
        [Route("/GenController/Presenters")]
        public IActionResult Presenters()
        {
            ViewBag.Images = new List<string>();
            foreach (string file in Directory.EnumerateFiles("wwwroot/css/images/Presenters/"))
            {
                string fileToAdd = file.Substring(30);
                fileToAdd = fileToAdd.Trim('/');
                ViewBag.Images.Add(fileToAdd);
            }
            ViewBag.Images.Sort();
            return View();
            //return RedirectToAction("ComingSoon");
        }

        [Route("~/Vendors")]
        [Route("/Home/Vendors")]
        [Route("/GenController/Vendors")]
        public IActionResult Vendors()
        {
            ViewBag.Images = new List<string>();
            foreach(string file in Directory.EnumerateFiles("wwwroot/css/images/Vendors/"))
            {
                string fileToAdd = file.Substring(27);
                fileToAdd = fileToAdd.Trim('/');
                ViewBag.Images.Add(fileToAdd);
            }
            ViewBag.Images.Sort();
            return View();
            //return RedirectToAction("ComingSoon");
        }

        [Route("~/Rules")]
        [Route("/Home/Rules")]
        [Route("/GenController/Rules")]
        public IActionResult Rules()
        {
            return View();
        }

        [Route("~/Schedule")]
        [Route("/Home/Schedule")]
        [Route("/GenController/Schedule")]
        public IActionResult Schedule()
        {
            ViewBag.Presentations = _kdbContext.Presentations.Where(p => p.EventId == 1111).ToList();
            foreach(Presentation presentation in ViewBag.Presentations)
            {
                presentation.Presenter = _kdbContext.Presenters.Where(p => p.Id == presentation.PresenterId).First();
            }
            return View();
        }

        [Route("~/Register")]
        [Route("~/Registration")]
        [Route("/Home/Registration")]
        [Route("/GenController/Registration")]
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
            if (goldCount > 0) { reg.TicketTypes.Add(new SelectListItem("Gold", "Gold")); }
            if (silverCount > 0) { reg.TicketTypes.Add(new SelectListItem("Silver", "Silver")); }
            reg.TicketTypes.Add(new SelectListItem("Standard", "Regular"));

            reg.RoomTypes = new List<SelectListItem>();
            reg.RoomTypes.Add(new SelectListItem("One King", "One King"));
            reg.RoomTypes.Add(new SelectListItem("Two Doubles", "Two Doubles"));
            reg.RoomTypes.Add(new SelectListItem("I will not be staying at the host hotel.", "I will not be staying at the host hotel."));
            reg.RoomTypes.Add(new SelectListItem("I am staying in someone else's room.", "I am staying in someone else's room."));

            if (goldCount == 0 && silverCount == 0 && regularCount == 0)
            {
                ViewBag.SoldOut = true;
                reg.WaitList = true;
            }

            return View(reg);
        }

        [Route("~/Register")]
        [Route("~/Registration")]
        [Route("/Home/Registration")]
        [Route("/GenController/Registration")]
        [HttpPost]
        public IActionResult Registration(RegistrationViewModel regUpdated)
        {
            if (!ModelState.IsValid)
            {


                ViewBag.Error = "Missing required info.";
                return View();
            }

            if (regUpdated.Email != regUpdated.EmailConf)
            {
                ViewBag.Error = "Email does not match.";
                return View();
            }

            if (regUpdated.DiscountCode is not null)
            {
                TicketComp? comp = _kdbContext.TicketComp
                    .Where(c => c.DiscountCode == regUpdated.DiscountCode)
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

            if (regUpdated.CreateMore == false)
            {
                if (regUpdated.WaitList == true)
                {
                    return Redirect("Waitlist");
                }
                return Redirect("InterstitialWait");
            }
            else
            {
                return Redirect("~/Register");
            }
        }

        [Route("~/Waitlist")]
        [Route("/Home/Waitlist")]
        [Route("/GenController/Waitlist")]
        public IActionResult Waitlist()
        {
            List<RegistrationViewModel> regList = GetRegFromCookies();
            _cookieService.DeleteCookie(_contextAccessor.HttpContext.Request, "Registration");
            WriteRegToDB(regList, null, null);
            return View();
        }

        [Route("~/InterstitialWait")]
        [Route("/Home/InterstitialWait")]
        [Route("/GenController/InterstitialWait")]
        public IActionResult InterstitialWait()
        {
            return View();
        }

        [Route("~/Payment")]
        [Route("/Home/Payment")]
        [Route("/GenController/Payment")]
        public IActionResult Payment()
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

            WriteRegToDB(regList, paymentURL.OrderId, paymentURL.Id);

            return Redirect(paymentURL.Url);
        }

        [Route("~/Privacy")]
        [Route("/Home/Privacy")]
        [Route("/GenController/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("~/Error")]
        [Route("/Home/Error")]
        [Route("/GenController/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new cure.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("~/Success")]
        [Route("~/Success/**")]
        [Route("/Home/Success")]
        [Route("/GenController/Success")]
        public IActionResult Success()
        {
            return View();
        }

        [Route("~/RegistrationSuccessful")]
        [Route("~/RegistrationSuccessful/**")]
        [Route("/Home/RegistrationSuccessful")]
        [Route("/GenController/RegistrationSuccessful")]
        public IActionResult RegSuccess()
        {
            List<RegistrationViewModel> registrationList = new List<RegistrationViewModel>();

            try
            {
                registrationList = GetRegFromCookies();
            }
            catch (ArgumentNullException ex)
            {
                //TODO: log
            }

            foreach (RegistrationViewModel registration in registrationList)
            {
                KiCData.Models.Member? member = _kdbContext.Members
                    .Where(m => m.FirstName == registration.FirstName && m.LastName == registration.LastName && m.DateOfBirth == registration.DateOfBirth)
                    .FirstOrDefault();

                if (member is not null)
                {
                    Attendee? attendee = _kdbContext.Attendees
                        .Where(a => a.MemberId == member.Id)
                        .FirstOrDefault();

                    if (attendee is not null) { attendee.IsPaid = true; }
                }
            }

            return View();
        }

        [Route("~/TicketTypes")]
        [Route("/Home/TicketTypes")]
        [Route("/GenController/TicketTypes")]
        public IActionResult TicketTypes()
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
            KiCData.Models.Event? CURE = _kdbContext.Events.Where(e => e.Name == "CURE").FirstOrDefault();
            foreach (var reg in regList)
            {
                if (reg.TicketComp is null)
                {
                    Ticket ticket = new Ticket()
                    {
                        EventId = CURE.Id,
                        Event = CURE,
                        Type = reg.TicketType,
                        DatePurchased = DateOnly.FromDateTime(DateTime.Now),
                        StartDate = CURE.StartDate,
                        EndDate = CURE.EndDate,
                        IsComped = reg.TicketComp is not null ? true : false,
                        Attendee = new Attendee()
                        {
                            BadgeName = reg.BadgeName,
                            BackgroundChecked = false,
                            ConfirmationNumber = 0,
                            RoomWaitListed = true,
                            TicketWaitListed = reg.WaitList,
                            RoomPreference = reg.RoomType,
                            IsPaid = false,
                            isRegistered = false,
                            Pronouns = reg.Pronouns,
                            OrderID = orderID,
                            PaymentLinkID = paymentLinkID,
                            Member = new KiCData.Models.Member()
                            {
                                FirstName = reg.FirstName,
                                LastName = reg.LastName,
                                Email = reg.Email,
                                DateOfBirth = reg.DateOfBirth,
                                FetName = reg.FetName,
                                ClubId = reg.ClubId,
                                PhoneNumber = reg.PhoneNumber,
                                SexOnID = reg.SexOnID,
                                City = reg.City,
                                State = reg.State
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

                    KiCData.Models.Member member = _kdbContext.Members
                        .Where(m => m.Id == attendee.MemberId)
                        .FirstOrDefault();

                    ticket.Price = 160;
                    ticket.Type = reg.TicketType;
                    ticket.DatePurchased = DateOnly.FromDateTime(DateTime.Now);
                    ticket.StartDate = CURE.StartDate;
                    ticket.EndDate = CURE.EndDate;
                    ticket.IsComped = true;

                    attendee.MemberId = member.Id;
                    attendee.TicketId = ticket.Id;
                    attendee.BadgeName = reg.BadgeName;
                    attendee.BackgroundChecked = false;
                    attendee.ConfirmationNumber = 0;
                    attendee.RoomWaitListed = true;
                    attendee.TicketWaitListed = reg.WaitList;
                    attendee.RoomPreference = reg.RoomType;
                    attendee.IsPaid = false;
                    attendee.isRegistered = false;
                    attendee.Pronouns = reg.Pronouns;
                    attendee.OrderID = orderID;
                    attendee.PaymentLinkID = paymentLinkID;

                    member.FirstName = reg.FirstName;
                    member.LastName = reg.LastName;
                    member.Email = reg.Email;
                    member.DateOfBirth = reg.DateOfBirth;
                    member.FetName = reg.FetName;
                    member.ClubId = reg.ClubId;
                    member.PhoneNumber = reg.PhoneNumber;
                    member.SexOnID = reg.SexOnID;
                    member.City = reg.City;
                    member.State = reg.State;

                    _kdbContext.SaveChanges();
                }

                if (reg.WillVolunteer == true)
                {
                    PendingVolunteer pVol = new PendingVolunteer()
                    {
                        FirstName = reg.FirstName,
                        LastName = reg.LastName,
                        Email = reg.Email,
                        Event = CURE,
                        EventId = CURE.Id
                    };

                    _kdbContext.PendingVolunteers.Add(pVol);
                    _kdbContext.SaveChanges();
                }
            }
        }
    }
}

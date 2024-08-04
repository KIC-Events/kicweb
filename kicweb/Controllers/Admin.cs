using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using KiCData.Models.WebModels;
using KiCData.Models;


namespace KiCWeb.Controllers
{
    [Route("[controller]")]
    public class Admin : Controller
    {

        private readonly ILogger<Admin> _logger;

        private readonly IUserService _userService;
        private KiCdbContext _context;

        public Admin(ILogger<Admin> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Registration",Name ="Registration")]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost] 
        [Route("Registration",Name ="Registration")]
        public IActionResult Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                KiCData.Models.Member registrant = _context.Members.FirstOrDefault(m => m.FirstName == model.FirstName && m.LastName == model.LastName && m.DateOfBirth == model.DateofBirth);
                if (registrant == null)
                {
                    return RedirectToAction("Index");
                }
                Attendee attendee = _context.Attendees.FirstOrDefault(a => a.MemberId == registrant.Id);
                if (attendee == null)
                {
                    return RedirectToAction("Index");
                }
                ConfirmationViewModel confirmation = new ConfirmationViewModel
                {
                    FirstName = registrant.FirstName,
                    LastName = registrant.LastName,
                    DateofBirth = registrant.DateOfBirth,
                    BadgeName = attendee.BadgeName
                };
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult TicketManagement()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TicketManagement(TicketViewModel ticket)
        {

            for (int i = 0; i < ticket.QtyTickets; i++)
            {
                Ticket newTicket = new Ticket
                {
                    Price = ticket.Price,
                    StartDate = ticket.StartDate,
                    EndDate = ticket.EndDate,
                    Type = ticket.Type
                };
                _context.Ticket.Add(newTicket);
            }
            _context.SaveChanges();

            return RedirectToAction("TicketManagement");
        }
    }
}

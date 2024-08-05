using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using KiCData.Models.WebModels;
using KiCData.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace KiCWeb.Controllers
{
    [Route("[controller]")]
    public class Admin : Controller
    {

        private readonly ILogger<Admin> _logger;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KiCdbContext _context;
        private readonly ICookieService _cookieService;

        public Admin(ILogger<Admin> logger, IConfigurationRoot configurationRoot, IUserService userService, IHttpContextAccessor httpContextAccessor, KiCdbContext kiCdbContext, ICookieService cookieService)
        {
            _logger = logger;
            _userService = userService;
            _configurationRoot = configurationRoot;
            _context = kiCdbContext;
            _contextAccessor = httpContextAccessor;
            _cookieService = cookieService;
        }

        //Shows Basic Admin Tasks for Registration, Tickets, Vendors, Volunteers, and Presenters
        [HttpGet]
        [Route("Admin")]
        public IActionResult AdminServices()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            
            return View();
        }

        //Request for adding tickets to the database for an event
        [HttpGet]
        [Route("Tickets")]
        public IActionResult AdminTickets()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            TicketViewModel ticket = new TicketViewModel
            {
                Events = _context.Events.Select(
                    a => new SelectListItem 
                    { 
                        Value = a.Id.ToString(),
                        Text = a.Name
                    }).ToList()
            };
            return View(ticket);
        }

        //Post request for adding tickets to the database for an event
        [HttpPost]
        public IActionResult AdminTickets(TicketViewModel ticket)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < ticket.QtyTickets; i++)
                {
                    Ticket ticket1 = new Ticket
                    {
                        Price = ticket.Price,
                        StartDate = ticket.StartDate,
                        EndDate = ticket.EndDate,
                        Type = ticket.Type
                    };
                    _context.Ticket.Add(ticket1);
                    _context.SaveChanges();
                    
                }
            }
            else
            {
                return View(ticket);
            }
            return RedirectToAction("Index");
        }

        //List of all events in the database
        [HttpGet]
        [Route("Events")]
        public IActionResult AdminEvents()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            IEnumerable<Event> events = _context.Events.ToList();
            return View(events);
        }

        //Details of a specific event
        [HttpGet]
        [Route("Events/{id}")]
        public IActionResult AdminEventDetails(int id)
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            Event events = _context.Events.Where(a => a.Id == id).FirstOrDefault();
            return View(events);
        }

        //Request for adding a new event to the database
        [HttpGet]
        [Route("Events/Create")]
        public IActionResult AdminEventCreate()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            
            EventViewModel evm = new EventViewModel
            {
                Venues = _context.Venue.Select(
                    a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    }).ToList()
            };

            return View(evm);
            
        }
        [HttpPost]
        public IActionResult AdminEventCreate(EventViewModel model) { 
            if (ModelState.IsValid)
            {
                Event ev = new Event
                {
                    Name = model.Name,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Topic = model.Topic,
                    Description = model.Description,
                    VenueId = model.VenueId
                };
                _context.Events.Add(ev);
                _context.SaveChanges();
                return RedirectToAction("AdminEventDetails", new {ev.Id});
            }
            else
            {
                return View(model);
            }
        }

        //[HttpGet]
        //public IActionResult Login()
        //{
        //
        //}
    }
}

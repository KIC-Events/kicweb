using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HostFiltering;
using KiCData.Models.WebModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;


namespace KiCWeb.Controllers
{
    [Route("[controller]")]
    public class Admin : KICAuthController
    {

        private readonly ILogger<Admin> _logger;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KiCdbContext _context;
        private readonly ICookieService _cookieService;

        public Admin(ILogger<Admin> logger, IConfigurationRoot configurationRoot, IUserService userService, IHttpContextAccessor httpContextAccessor, KiCdbContext kiCdbContext, ICookieService cookieService) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
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
        [Route("~/Admin/AdminServices")]
        public IActionResult AdminServices()
        {
            return View();
        }

        [HttpGet]
        [Route("/Admin/Registration")]
        [Route("/Registration")]
        public IActionResult Registration()
        {
            CheckInViewModel cvm = new CheckInViewModel(_context);

            return View(cvm);
        }

        //Request for adding tickets to the database for an event
        [HttpGet]
        [Route("Tickets")]
        public IActionResult AdminTickets()
        {


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
            IEnumerable<Event> events = _context.Events.Include(b => b.Venue).ToList();
            return View(events);
        }

        //Details of a specific event
        [HttpGet]
        [Route("Events/{id}")]
        public IActionResult AdminEventDetails(int id)
        {
            Event events = _context.Events.Where(a => a.Id == id).FirstOrDefault();
            events.Venue = _context.Venue.Where(a => a.Id == events.VenueId).FirstOrDefault();
            events.Tickets = _context.Ticket.Where(a => a.EventId == id).ToList();
            
            return View(events);
        }

        //Request for adding a new event to the database
        [HttpGet]
        [Route("Events/Create")]
        public IActionResult AdminEventCreate()
        {
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
        //Post request for adding a new event to the database
        [HttpPost]
        public IActionResult AdminEventCreate(EventViewModel model) 
        {
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

        //Request for editing an event in the database
        [HttpGet]
        [Route("Events/Edit/{id}")]
        public IActionResult AdminEventEdit(int id)
        {
            Event events = _context.Events.Where(a => a.Id == id).FirstOrDefault();
            
            return View(events);
        }
        /*
         * Section for Volunteers, assigning pending volunteers to specific roles/shifts
         * add volunteer to event, remove volunteer from event
         */
        //Lists all volunteers that submitted for a specific event
        [HttpGet]
        [Route("Volunteers")]
        public IActionResult PendingVolunteerIndex()
        {
            IEnumerable<PendingVolunteer> pending = _context.PendingVolunteers.ToList();
            return View(pending);
        }

        //Approves and assigns a volunteer to a specific role/shift
        [HttpGet]
        [Route("Volunteers/Approve/{id}")]
        public IActionResult VolunteerApproval(int id)
        {
            PendingVolunteer pending = _context.PendingVolunteers.Where(a => a.Id == id).FirstOrDefault();
            if (pending == null)
            {
                return RedirectToAction("PendingVolunteerIndex");
            }
            else
            {
                //EventVolunteerViewModel eVVm = new EventVolunteerViewModel
                //{
                //    VolunteerId = pending.VolunteerID,
                //    VolunteerName = pending.Volunteer.Member.FetName,
                //    EventId = pending.EventId,
                //    EventName = pending.Event.Name,
                //    Positions = pending.PreferredPositions.Split(',').Select(a => new SelectListItem { Value = a, Text = a }).ToList()
                //};
            }
            
            return View(pending);
        }
        [HttpPost]
        public IActionResult VolunteerApproval(EventVolunteerViewModel eventVolunteer)
        {
            if (ModelState.IsValid)
            {
                EventVolunteer volunteer = new EventVolunteer
                {
                    VolunteerId = eventVolunteer.VolunteerId,
                    EventId = eventVolunteer.EventId,
                    Position = eventVolunteer.Position,
                    ShiftStart = eventVolunteer.ShiftStart,
                    ShiftEnd = eventVolunteer.ShiftEnd
                };
                return RedirectToAction("PendingVolunteerIndex");
            }
            ViewBag.Error = "There is a problem validating this information. Please review";
            return View(eventVolunteer);
        }
        // end of volunteer section //


        /*
         * Section for Vendors, Index, Details, Create, Edit, Delete
         */
        [Route("Vendors")]
        public IActionResult VendorIndex()
        {
            IEnumerable<Vendor> vendors = _context.Vendors.ToList();
            return View(vendors);
        }

        public IActionResult VendorDetails(int id)
        {
            Vendor vendor = _context.Vendors.Where(a => a.Id == id).FirstOrDefault();
            if (vendor == null) 
            {
                ViewBag.Error = "Vendor not found";
                return RedirectToAction("VendorIndex");
            }
            return View(vendor);
        }


        // End of Vendor Section //
        //[HttpGet]
        //public IActionResult Login()
        //{
        //
        //}
    }
}

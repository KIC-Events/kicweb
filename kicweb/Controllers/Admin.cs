using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using KiCData.Models.WebModels;
using KiCData.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;


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

        //[HttpGet]
        //public IActionResult Login()
        //{
        //
        //}
    }
}

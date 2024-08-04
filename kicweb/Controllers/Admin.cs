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

        public Admin(ILogger<Admin> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Registration",Name ="Registration")]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TicketManagement()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TicketManagement(TicketViewModel ticket)
        {

            using (var context = new KiCdbContext(options))
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
                    context.Tickets.add(newTicket);
                }
                context.SaveChanges();
            }
            return RedirectToAction("TicketManagement");
        }
    }
}

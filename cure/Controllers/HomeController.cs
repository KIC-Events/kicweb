using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using cure.Models;
using Cure.Models;


namespace cure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IConfigurationRoot _config;

        public HomeController(ILogger<HomeController> logger, IPaymentService paymentService, IConfigurationRoot configuration)
        {
            _logger = logger;
            _paymentService = paymentService;
            _config = configuration;
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
            try
            {
                silverCount = _paymentService.CheckInventory("CURE Event Ticket", "Silver");
                goldCount = _paymentService.CheckInventory("CURE Event Ticket", "Gold");
            }
            catch (Exception ex)
            {
                return Redirect("Error");
                //TODO Log exception
            }
            
            RegistrationViewModel reg = new RegistrationViewModel();
            reg.TicketTypes = new List<SelectListItem>();
            reg.TicketTypes.Add(new SelectListItem("Gold - " + goldCount.ToString() + " Remaining", "Gold"));
            reg.TicketTypes.Add(new SelectListItem("Silver- " + silverCount.ToString() + " Remaining", "Silver"));
            reg.TicketTypes.Add(new SelectListItem("Early Pricing - Standard", "Standard"));
            reg.RoomTypes = new List<SelectListItem>();
            reg.RoomTypes.Add(new SelectListItem("One King", "One King"));
            reg.RoomTypes.Add(new SelectListItem("Two Doubles", "Two Doubles"));
            reg.RoomTypes.Add(new SelectListItem("I will not be staying at the host hotel.", "I will not be staying at the host hotel."));
            reg.VolunteerPositions = new List<VolunteerPositionSelection>();
            reg.VolunteerPositions.Add(new VolunteerPositionSelection("DM"));
            reg.VolunteerPositions.Add(new VolunteerPositionSelection("Hallway Monitor"));
            reg.VolunteerPositions.Add(new VolunteerPositionSelection("Registration"));
            reg.VolunteerPositions.Add(new VolunteerPositionSelection("I do not wish to volunteer."));
            return View(reg);
        }

        [HttpPost]
        public IActionResult Registration(RegistrationViewModel regUpdated)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Missing required info.";
                return View(regUpdated);
            }

            return Redirect("Success");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new cure.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}

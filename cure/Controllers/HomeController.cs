using cure.Models;
using KiCData.Models;
using KiCData.Models.WebModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace cure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("~/")]
        [Route("~/Home")]
        [Route("~/Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Presenters()
        {
            return View();
        }

        public IActionResult Vendors()
        {
            return View();
        }

        public IActionResult Rules()
        {
            return View();
        }

        public IActionResult Schedule()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registration()
        {
            RegistrationViewModel reg = new RegistrationViewModel();
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

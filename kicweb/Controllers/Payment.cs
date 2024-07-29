using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KiCWeb.Controllers
{
    [Route("[controller]")]
    public class Payment : Controller
    {
        private readonly ILogger<Payment> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICookieService _cookieService;
        private readonly IConfigurationRoot _configurationRoot;

        public Payment(ILogger<Payment> logger, IHttpContextAccessor contextAccessor, ICookieService cookieService, IConfigurationRoot configurationRoot)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _cookieService = cookieService;
            _configurationRoot = configurationRoot;
        }

        [HttpGet("Purchase")]
        public IActionResult Purchase()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~/Home/Index");
            }

            return View();
        }

        [HttpGet("Merch")]
        [Route("/Merch")]
        public IActionResult MerchStore()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~/Home/Index");
            }

            return View("Merch");
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View("Error!");
        //}
    }
}
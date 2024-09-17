using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KiCWeb.Controllers
{
    [Route("[controller]")]
    public class Payment : KICController
    {
        private readonly ILogger<Payment> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICookieService _cookieService;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly KiCdbContext _context;

        public Payment(ILogger<Payment> logger, IHttpContextAccessor contextAccessor, ICookieService cookieService, IConfigurationRoot configurationRoot, KiCdbContext kiCdbContext) : base(configurationRoot, userService: null, contextAccessor, kiCdbContext, cookieService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _cookieService = cookieService;
            _configurationRoot = configurationRoot;
            _context = kiCdbContext;
        }

        [HttpGet("Purchase")]
        public IActionResult Purchase()
        {
            return View();
        }

        [HttpGet("Merch")]
        [Route("/Merch")]
        public IActionResult MerchStore()
        {
            return Redirect("https://kic-events.square.site/shop/apparel/INJSIHWIBYY7LG4HENI4NYFL");
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View("Error!");
        //}
    }
}
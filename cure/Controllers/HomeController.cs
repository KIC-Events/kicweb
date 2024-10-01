using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using cure.Models;
using Cure.Models;
using Square;
using KiCData.Models.WebModels.Member;
using Newtonsoft.Json;
using Square.Models;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Asn1.Mozilla;


namespace cure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IConfigurationRoot _config;
        private readonly ICookieService _cookieService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KiCdbContext _kdbContext;

        public HomeController(ILogger<HomeController> logger, IPaymentService paymentService, IConfigurationRoot configuration, ICookieService cookieService, IHttpContextAccessor contextAccessor, KiCdbContext kiCdbContext, IUserService userService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _config = configuration;
            _cookieService = cookieService;
            _contextAccessor = contextAccessor;
            _kdbContext = kiCdbContext;
        }

        [Route("~/")]
        [Route("~/Home")]
        [Route("~/Home/Index")]
        [Route("**")]
        [HttpGet]
        public IActionResult Index()
        {
            _cookieService.DeleteCookie(_contextAccessor.HttpContext.Request, "Registration");

            IndexViewModel ivm = new IndexViewModel()
            {
                Consent = false
            };

            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                //ivm.Consent = false;
                ViewBag.AgeGateCookieAccepted = false;
            }
            else { ViewBag.AgeGateCookieAccepted = true; }

            return View(ivm);
        }

        [HttpPost]
        public IActionResult Index(IndexViewModel ivmUpdated)
        {
            _cookieService.DeleteCookie(_contextAccessor.HttpContext.Request, "Registration");

            if (ivmUpdated.Consent == true)
            {
                ViewBag.AgeGateCookieAccepted = true;
                CookieOptions cookieOptions = _cookieService.NewCookieFactory();
                _contextAccessor.HttpContext.Response.Cookies.Append("Age_Gate", "true", cookieOptions);

                return View();
            }
            else
            {
                ViewBag.AgeGateCookieAccepted = false;
                return View();
            }
        }

        
    }
}

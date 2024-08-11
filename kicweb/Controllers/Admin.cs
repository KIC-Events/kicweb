using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;

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
                return Redirect("~/Home/Index");
            }

            if (!_cookieService.AuthTokenCookie(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~/Member/Login");
            }

            return View();
        }

        //[HttpGet]
        //public IActionResult Login()
        //{
        //
        //}
    }
}

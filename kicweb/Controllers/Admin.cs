using KiCData.Services;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpGet]
        //public IActionResult Login()
        //{
        //
        //}
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using KiCData;
using KiCData.Services;
using KiCData.Models.WebModels;
using KiCData.Models;

namespace KiCWeb.Controllers
{
    [Route("cure")]
    public class CureController : KICController
    {
        public CureController(
            IConfigurationRoot configurationRoot,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            KiCdbContext kiCdbContext,
            ICookieService cookieService
        ) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
        }

        [Route("")]
        public IActionResult Index()
        {
            return View(); // Views/Cure/Index.cshtml
        }

        [Route("registration")]
        public IActionResult Registration()
        {
            return View(); // Views/Cure/Registration.cshtml
        }

        [Route("rules")]
        public IActionResult Rules()
        {
            return View(); // Views/Cure/Rules.cshtml
        }

        [Route("presenters")]
        public IActionResult Presenters()
        {
            return View(); // Views/Cure/Presenters.cshtml
        }

        [Route("volunteers")]
        public IActionResult Volunteers()
        {
            return View(); // Views/Cure/Volunteers.cshtml
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using KiCData;
using KiCData.Services;
using KiCData.Models.WebModels;
using KiCData.Models;
using System.Text.Json;

namespace KiCWeb.Controllers
{
    [Route("cure")]
    public class CureController : KICController
    {
        private readonly KiCdbContext _kdbContext;

        public CureController(
            IConfigurationRoot configurationRoot,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            KiCdbContext kiCdbContext,
            ICookieService cookieService
        ) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
            _kdbContext = kiCdbContext ?? throw new ArgumentNullException(nameof(kiCdbContext));
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

        [Route("registration/form")]
        public IActionResult RegistrationForm()
        {
            // This action could be used to return a form for registration
            // You might want to return a partial view or a specific view for the form
            return View(); // Views/Cure/RegistrationForm.cshtml
        }

        [Route("rules")]
        public IActionResult Rules()
        {
            return View(); // Views/Cure/Rules.cshtml
        }

        [Route("presenters")]
        public IActionResult Presenters()
        {
            var presenters = _kdbContext.Presenters
                .OrderBy(p => p.PublicName)
                .ToList();

            ViewBag.Presenters = presenters;

            // Log to console or logger
            string json = JsonSerializer.Serialize(presenters, new JsonSerializerOptions
            {
                WriteIndented = true // optional: makes it pretty
            });
            Console.WriteLine("Presenters JSON:");
            Console.WriteLine(json); // or use your logger
            
            return View(); // Views/Cure/Presenters.cshtml
        }

        [Route("volunteers")]
        public IActionResult Volunteers()
        {
            return View(); // Views/Cure/Volunteers.cshtml
        }
    }
}
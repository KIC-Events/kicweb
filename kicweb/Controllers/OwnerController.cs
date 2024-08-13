using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using KiCData.Models.WebModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace KiCWeb.Controllers
{
    public class OwnerController : Controller
    {
        private readonly ILogger<Admin> _logger;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KiCdbContext _context;
        private readonly ICookieService _cookieService;

        public OwnerController(ILogger<Admin> logger, IConfigurationRoot configurationRoot, IUserService userService, IHttpContextAccessor httpContextAccessor, KiCdbContext kiCdbContext, ICookieService cookieService)
        {
            _logger = logger;
            _userService = userService;
            _configurationRoot = configurationRoot;
            _context = kiCdbContext;
            _contextAccessor = httpContextAccessor;
            _cookieService = cookieService;
        }
        public IActionResult Index()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            return View();
        }

        public IActionResult CompIndex()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            IEnumerable<TicketComp> comps = _context.TicketComp.ToList();
            return View(comps);
        }

        [HttpGet]
        public IActionResult CompAdd()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult CompAdd(CompViewModel comp)
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            if (ModelState.IsValid)
            {
                for (int i = 0; i < comp.CompQuantity; i++)
                {
                    _context.TicketComp.Add(new TicketComp { CompAmount = comp.CompAmount, CompReason = comp.CompReason, AuthorizingUser = comp.AuthorizingUser, DiscountCode = comp.DiscountCode});
                    _context.SaveChanges();
                    
                }
                
            }
            return RedirectToAction("CompIndex");
            //return View(comp);
        }

        [HttpGet]
        public IActionResult CompEdit(int id)
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            var comp = _context.TicketComp.Find(id);
            if (comp == null)
            {
                return RedirectToAction("CompIndex");
            }
            return View(comp);
        }

        [HttpPost]
        public IActionResult CompEdit(TicketComp comp)
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            if (ModelState.IsValid)
            {
                _context.TicketComp.Update(comp);
                _context.SaveChanges();
                return RedirectToAction("CompIndex");
            }
            return View(comp);
        }

        //[HttpPost]
        public IActionResult CompDelete(int id) 
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            TicketComp comp = _context.TicketComp.Find(id);
            if (comp != null) 
            {
                _context.Remove(comp);
                _context.SaveChanges();
                RedirectToAction("CompIndex"); 
            }
            ViewBag.ErrorMessage = "Comp not found";
            return RedirectToAction("CompIndex");

        }

    }
}

using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace KiCWeb.Controllers
{
    public class KICAuthController : KICController
    {
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KiCdbContext _context;
        private readonly ICookieService _cookieService;

        public KICAuthController(IConfigurationRoot configurationRoot, IUserService userService, IHttpContextAccessor httpContextAccessor, KiCdbContext kiCdbContext, ICookieService cookieService) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
            _userService = userService;
            _configurationRoot = configurationRoot;
            _context = kiCdbContext;
            _contextAccessor = httpContextAccessor;
            _cookieService = cookieService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (!_cookieService.AuthTokenCookie(_contextAccessor.HttpContext.Request))
            {
                context.Result = new RedirectToActionResult("Login", "Member", context.HttpContext.Request.RouteValues);
            }

            base.OnActionExecuting(context);
        }
    }
}

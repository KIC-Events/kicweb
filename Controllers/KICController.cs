using System.Net;
using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiCWeb.Controllers
{
	public class KICController : Controller
	{
		private readonly IConfigurationRoot _configurationRoot;
		private readonly IUserService _userService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly KiCdbContext _context;
		private readonly ICookieService _cookieService;

		public KICController(IConfigurationRoot configurationRoot, IUserService userService, IHttpContextAccessor httpContextAccessor, KiCdbContext kiCdbContext, ICookieService cookieService)
		{
			_userService = userService;
			_configurationRoot = configurationRoot;
			_context = kiCdbContext;
			_contextAccessor = httpContextAccessor;
			_cookieService = cookieService;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var regService = context.HttpContext.RequestServices.GetRequiredService<RegistrationSessionService>();
			if (!regService.IsEmpty())
			{
				ViewBag.checkoutCount = regService.Registrations.Count;
			}
			
			if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request) &&
			    (context.RouteData.Values["controller"] ?? "").ToString() != "Home") //don't redirect if we're in HomeController
			{
				CookieOptions options = _cookieService.NewCookieFactory();
				_contextAccessor.HttpContext.Response.Cookies.Append("age_gate_redirect", context.HttpContext.Request.Path, options);
				context.Result = new RedirectToActionResult("Index", "Home", context.HttpContext.Request.RouteValues);
			}

			base.OnActionExecuting(context);
		}
	}
}

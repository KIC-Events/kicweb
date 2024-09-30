using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using KiCData;
using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Org.BouncyCastle.Crypto.Fpe;
using Microsoft.AspNetCore.Authorization;

namespace KiCWeb.Controllers;

public class HomeController : Controller
{
	private readonly IKiCLogger _logger;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly ICookieService _cookieService;
	private readonly IEmailService _emailService;
	private readonly IConfigurationRoot _configurationRoot;
	private KiCdbContext _kdbContext;

	public HomeController(IKiCLogger logger, IHttpContextAccessor httpContextAccessor, ICookieService cookieService, IEmailService emailService, IConfigurationRoot configurationRoot, KiCdbContext kiCdbContext)
	{
		_logger = logger;
		_contextAccessor = httpContextAccessor;
		_cookieService = cookieService;
		_emailService = emailService;
		_configurationRoot = configurationRoot;
		_kdbContext = kiCdbContext;
	}

	[HttpGet]
	public IActionResult Index()
	{
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

		List<Event> events = _kdbContext.Events
			.Where(e => e.StartDate > DateOnly.FromDateTime(DateTime.Now))
			.ToList();
		ViewBag.Events = events;

        return View(ivm);
	}

	[HttpPost]
	public IActionResult Index(IndexViewModel ivmUpdated)
	{
		if (ivmUpdated.Consent == true)
		{
			ViewBag.AgeGateCookieAccepted = true;
			CookieOptions cookieOptions = _cookieService.NewCookieFactory();
			_contextAccessor.HttpContext.Response.Cookies.Append("Age_Gate", "true", cookieOptions); 
			
			List<Event> events = _kdbContext.Events
				.Where(e => e.StartDate > DateOnly.FromDateTime(DateTime.Now))
				.ToList();

            ViewBag.Events = events;
            return View();
		}
		else
		{
			ViewBag.AgeGateCookieAccepted = false;

            List<Event> events = _kdbContext.Events
                .Where(e => e.StartDate > DateOnly.FromDateTime(DateTime.Now))
                .ToList();

            ViewBag.Events = events;

            return View();
		}
	}

	
}

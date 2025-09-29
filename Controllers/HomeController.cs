using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using Org.BouncyCastle.Crypto.Fpe;
using Microsoft.AspNetCore.Authorization;
using KiCData;
using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace KiCWeb.Controllers;

public class HomeController : KICController
{
	private readonly IKiCLogger _logger;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly ICookieService _cookieService;
	private readonly IEmailService _emailService;
	private readonly IConfigurationRoot _configurationRoot;
	private KiCdbContext _kdbContext;

	public HomeController(IKiCLogger logger, IHttpContextAccessor httpContextAccessor, ICookieService cookieService, IEmailService emailService, IConfigurationRoot configurationRoot, KiCdbContext kiCdbContext, IHttpContextAccessor contextAccessor): base(configurationRoot, userService: null, contextAccessor, kiCdbContext, cookieService)
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

		List<Event> events = _kdbContext.Events
			.Where(e => e.StartDate > DateOnly.FromDateTime(DateTime.Now))
			.ToList();
		
		List<Event> eventsWithImages = new List<Event>();
		List<Event> eventsWithoutImages = new List<Event>();
		
		foreach(Event e in events)
		{
			if(e.ImagePath is not null)
			{
				eventsWithImages.Add(e);
			}
			else
			{
				eventsWithoutImages.Add(e);
			}
		}
		
		ViewBag.events = eventsWithImages; // New events for redesign (TODO refactor this)
		ViewBag.EventsWithImages = eventsWithImages;
		ViewBag.EventsWithoutImages = eventsWithoutImages;

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
			
			if(_contextAccessor.HttpContext.Request.Cookies["age_gate_redirect"] is not null)
			{
				string path = _contextAccessor.HttpContext.Request.Cookies["age_gate_redirect"];
				_contextAccessor.HttpContext.Response.Cookies.Delete("age_gate_redirect");
				return Redirect(path);
			}
			
			List<Event> events = _kdbContext.Events
			.Where(e => e.StartDate > DateOnly.FromDateTime(DateTime.Now))
			.ToList();
		
			List<Event> eventsWithImages = new List<Event>();
			List<Event> eventsWithoutImages = new List<Event>();
			
			foreach(Event e in events)
			{
				if(e.ImagePath is not null)
				{
					eventsWithImages.Add(e);
				}
				else
				{
					eventsWithoutImages.Add(e);
				}
			}
			
			ViewBag.EventsWithImages = eventsWithImages;
			ViewBag.EventsWithoutImages = eventsWithoutImages;
			return View();
		}
		else
		{
			ViewBag.AgeGateCookieAccepted = false;

			List<Event> events = _kdbContext.Events
			.Where(e => e.StartDate > DateOnly.FromDateTime(DateTime.Now))
			.ToList();
		
			List<Event> eventsWithImages = new List<Event>();
			List<Event> eventsWithoutImages = new List<Event>();
			
			foreach(Event e in events)
			{
				if(e.ImagePath is not null)
				{
					eventsWithImages.Add(e);
				}
				else
				{
					eventsWithoutImages.Add(e);
				}
			}
			
			ViewBag.EventsWithImages = eventsWithImages;
			ViewBag.EventsWithoutImages = eventsWithoutImages;

			return View();
		}
	}

	
}

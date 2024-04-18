using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kicweb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using kicweb.Services;
using System.Net;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using KiCData;
using KiCData.Models;

namespace KiCWeb.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly ICookieService _cookieService;
	private readonly IEmailService _emailService;
	private readonly IConfigurationRoot _configurationRoot;

	public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, ICookieService cookieService, IEmailService emailService, IConfigurationRoot configurationRoot)
	{
		_logger = logger;
		_contextAccessor = httpContextAccessor;
		_cookieService = cookieService;
		_emailService = emailService;
		_configurationRoot = configurationRoot;
	}

	[HttpGet]
	public IActionResult Index()
	{
		IndexViewModel ivm = new IndexViewModel()
		{
			Consent = true
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
		if (ivmUpdated.Consent == true)
		{
			ViewBag.AgeGateCookieAccepted = true;
			CookieOptions cookieOptions = _cookieService.AgeGateCookieFactory();
			_contextAccessor.HttpContext.Response.Cookies.Append("Age_Gate", "true", cookieOptions);
			return View();
		}
		else
		{
			ViewBag.AgeGateCookieAccepted = false;
			return View();
		}
	}

	public IActionResult Privacy()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View();
	}

	public IActionResult Club425()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View();
	}

	public IActionResult About()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View();
	}

	public IActionResult Events()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Purchase()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Presenters()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Vendors()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	[HttpGet]
	public IActionResult Volunteers()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        ViewBag.PositionList = GetPositions();
		Volunteer volunteer = new Volunteer();

        return View(volunteer);
	}

	[HttpPost]
	public IActionResult Volunteers(Volunteer volUpdated)
	{
		if(!ModelState.IsValid)
		{
			return View("Volunteers");
		}

		MimeMessage message = _emailService.FormSubmissionEmailFactory("Volunteer", _configurationRoot["Email Addresses:Volunteers"].ToString());
		if (message == null)
		{
			//log exception here

			return Redirect("Error");
		}

		/*
		StringBuilder posList = new StringBuilder();
		foreach(string s in vvmUpdated.Positions)
		{
			posList.Append(s + ", ");
		}
		*/		

		message.Body = new TextPart("html")
		{
			Text = "<p>This is an automated email message sent through kicevents.com. A new volunteer sign up has occurred.</p>" +
			"<br />" +
			"<br />" +
            "<br /><b>Name: </b>" + volUpdated.LegalName +
            "<br /><b>Fet Name: </b>" + volUpdated.FetName +
			"<br /><b>Club ID: </b>" + volUpdated.ClubId +
            "<br /><b>Email: </b>" + volUpdated.EmailAddress +
            "<br /><b>Details: </b>" + volUpdated.Details +
            //"<br /><bPositions: </b>" + posList.ToString() +
            "<br />" +
            "<br />" +
			"Please take any necessary action from here. If you encounter issues with this email, or you believe it has been sent in error, please reply to it."
        };

		try
		{
            _emailService.SendEmail(message);
        }
		catch (Exception ex)
		{
			//Log exception here
			return Redirect("Error");
		}

		return Redirect("Success");
	}

	/// <summary>
	/// Gets the available positions for which user can volunteer.
	/// </summary>
	/// <returns>MultiSelectList</returns>
	private MultiSelectList GetPositions()
	{
		List<Position> positions = new List<Position>()
		{
			new Position(){ID=1, Name="Bartender" },
			new Position(){ID=2, Name="Door"},
			new Position(){ID=3, Name="DM"},
			new Position(){ID=4, Name="Corporal"},
			new Position(){ID=5, Name="Fire"},
			new Position(){ID=6, Name="Electric"}
		};

		return new MultiSelectList(positions, "ID", "Name", null);
	}

	public IActionResult Contact()
	{
		return View();
	}

	public IActionResult Success()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}

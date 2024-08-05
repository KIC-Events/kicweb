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
			Consent = true
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
			CookieOptions cookieOptions = _cookieService.AgeGateCookieFactory();
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

	public IActionResult Purchase()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	
    //Issue #86 https://github.com/Malechus/kic/issues/86
    /*
	[HttpGet]
	public IActionResult Presenters()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
		ViewBag.Error = null;
		Presentation presentation = new Presentation() { Presenter = new Presenter() };
		return View(presentation);
	}

	[HttpPost]
	public IActionResult Presenters(Presentation presUpdated)
	{
		if (!ModelState.IsValid)
		{
			ViewBag.Error = "There was a validation issue.";
			return Redirect("Presenters");
		}

		FormMessage formMessage = _emailService.FormSubmissionEmailFactory("Presenters");
		if(formMessage is null)
		{
			//log error here

			return Redirect("Error");
		}

        formMessage.HtmlBuilder.Append("<p>This is an automated email message sent through kicevents.com. A new presentation sign up has occurred.</p>" +
            "<br />" +
            "<br />" +
            "<br /><b>FetName: </b>" + presUpdated.Presenter.Member.FetName +
            "<br /><b>Business Name: </b>" + presUpdated.Presenter.PublicName +
            "<br /><b>Email: </b>" + presUpdated.Presenter.Member.EmailAddress +
            "<br /><b>Presenter Details: </b>" + presUpdated.Presenter.Bio +
			"<br /><b>Presentation Name: </b>" + presUpdated.Name +
            "<br /><b>Presentation Description: </b>" + presUpdated.Description +
            "<br /><b>Presentation Topic: </b>" + presUpdated.Type +
            "<br />" +
            "<br />" +
            "Please take any necessary action from here. If you encounter issues with this email, or you believe it has been sent in error, please reply to it."
        );

        try
        {
            _emailService.SendEmail(formMessage);
        }
        catch (Exception ex)
        {
            _logger.Log(ex, _contextAccessor.HttpContext.Request);
            return Redirect("Error");
        }

        return Redirect("Success");
    }

	[HttpGet]
	public IActionResult Vendors()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
		ViewBag.Error = null;
		Vendor vendor = new Vendor();
		return View(vendor);
	}

	[HttpPost]
	public IActionResult Vendors(Vendor venUpdated)
	{
		if (!ModelState.IsValid)
		{
			ViewBag.Error = "There was a validation issue.";
			return View("Vendors");
		}

		FormMessage formMessage = _emailService.FormSubmissionEmailFactory("Vendors");
		if(formMessage == null)
		{
			//log exception here

			return Redirect("Error");
		}

		formMessage.HtmlBuilder.Append("<p>This is an automated email message sent through kicevents.com. A new vendor sign up has occurred.</p>" +
            "<br />" +
            "<br />" +
            "<br /><b>FetName: </b>" + venUpdated.Member.FetName +
			"<br /><b>Business Name: </b>" + venUpdated.PublicName +
            "<br /><b>Email: </b>" + venUpdated.Member.EmailAddress +
            "<br /><b>Details: </b>" + venUpdated.Bio +
            "<br />" +
            "<br />" +
            "Please take any necessary action from here. If you encounter issues with this email, or you believe it has been sent in error, please reply to it."
        );

		try
		{
			_emailService.SendEmail(formMessage);
        }
        catch (Exception ex)
        {
            _logger.Log(ex, _contextAccessor.HttpContext.Request);
            return Redirect("Error");
        }

        return Redirect("Success");
    }

	[HttpGet]
	public IActionResult Volunteers()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
		ViewBag.Error = null;
		Volunteer volunteer = new Volunteer();

        return View(volunteer);
    }

	[HttpPost]
	public IActionResult Volunteers(Volunteer volUpdated)
	{
		if(!ModelState.IsValid)
		{
			ViewBag.Error = "There was a validation issue.";
			return View("Volunteers");
		}

		FormMessage message = _emailService.FormSubmissionEmailFactory("Volunteers");
		if (message == null)
		{
			//log exception here

			return Redirect("Error");
		}

		message.HtmlBuilder.Append("<p>This is an automated email message sent through kicevents.com. A new volunteer sign up has occurred.</p>" +
			"<br />" +
			"<br />" +
            "<br /><b>Name: </b>" + volUpdated.Member.LegalName +
            "<br /><b>Fet Name: </b>" + volUpdated.Member.FetName +
			"<br /><b>Club ID: </b>" + volUpdated.Member.ClubId +
            "<br /><b>Email: </b>" + volUpdated.Member.EmailAddress +
            "<br /><b>Phone: </b>" + volUpdated.Member.PhoneNumber +
            "<br /><b>Details: </b>" + volUpdated.Details +
            "<br />" +
            "<br />" +
			"Please take any necessary action from here. If you encounter issues with this email, or you believe it has been sent in error, please reply to it."
        );

		try
		{
            _emailService.SendEmail(message);
        }
		catch (Exception ex)
		{
			_logger.Log(ex, _contextAccessor.HttpContext.Request);
			return Redirect("Error");
		}

		return Redirect("Success");
	}
	

    [HttpGet]
	public IActionResult Contact()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        ViewBag.Error = null;
		Feedback feedback = new Feedback();

		return View(feedback);
    }

	[HttpPost]
	public IActionResult Contact(Feedback feedbackUpdated)
	{
		if (!ModelState.IsValid)
		{
			ViewBag.Error = "There was a validation issue.";
			return View("Contact");
		}

        FormMessage message = _emailService.FormSubmissionEmailFactory("Admin");
        if (message == null)
        {
			_logger.Log(new Exception("The message is null."), _contextAccessor.HttpContext.Request);

            return Redirect("Error");
        }

        message.HtmlBuilder.Append("<p>This is an automated email message sent through kicevents.com. A new feedback submission has occurred.</p>" +
            "<br />" +
            "<br />" +
            "<br /><b>Details: </b>" + feedbackUpdated.Text +
            "<br />" +
            "<br />" +
            "Please take any necessary action from here. If you encounter issues with this email, or you believe it has been sent in error, please reply to it."
        );

        try
        {
            _emailService.SendEmail(message);
        }
        catch (Exception ex)
        {
            _logger.Log(ex, _contextAccessor.HttpContext.Request);
            return Redirect("Error");
        }

        return Redirect("Success");
    }
	*/

	public IActionResult Contact()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
        return View();
    }

	public IActionResult GetInvolved()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Index");
        }
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

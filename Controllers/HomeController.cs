using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kicweb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using kicweb.Services;
using System.Net;

namespace kicweb.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly ICookieService _cookieService;

	public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, ICookieService cookieService)
	{
		_logger = logger;
		_contextAccessor = httpContextAccessor;
		_cookieService = cookieService;
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
            return Redirect("Home/Index");
        }
        return View();
	}

	public IActionResult Club425()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Home/Index");
        }
        return View();
	}

	public IActionResult About()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Home/Index");
        }
        return View();
	}

	public IActionResult Events()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Home/Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Purchase()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Home/Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Presenters()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Home/Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Vendors()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Home/Index");
        }
        return View("/Views/Shared/UnderConstruction.cshtml");
	}

	[HttpGet]
	public IActionResult Volunteers()
	{
        if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
        {
            return Redirect("Home/Index");
        }
        ViewBag.PositionList = GetPositions();
		VolViewModel vvm = new VolViewModel();
		return View(vvm);
	}

	[HttpPost]
	public IActionResult Volunteers(VolViewModel vvmUpdated)
	{

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

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}

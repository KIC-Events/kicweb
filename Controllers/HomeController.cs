using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kicweb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace kicweb.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
	}

	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	public IActionResult Club425()
	{
		return View();
	}

	public IActionResult About()
	{
		return View();
	}

	public IActionResult Events()
	{
		return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Purchase()
	{
		return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Presenters()
	{
		return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Vendors()
	{
		return View("/Views/Shared/UnderConstruction.cshtml");
	}

	public IActionResult Volunteers()
	{
		ViewBag.PositionList = GetPositions();
		return View();
	}

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

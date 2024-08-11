using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KiCData.Services;
using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Models.WebModels.Member;
using System.Text;
using System.Security.Cryptography;

namespace KiCWeb.Controllers
{
	[Route("[controller]")]
	public class Member : Controller
	{
		private readonly ILogger<Member> _logger;
		private readonly ICookieService _cookieService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IUserService _userService;
		private readonly KiCdbContext _context;

		public Member(ILogger<Member> logger, IUserService userService, ICookieService cookieService, IHttpContextAccessor contextAccessor, KiCdbContext kiCdbContext)
		{
			_logger = logger;
			_userService = userService;
			_cookieService = cookieService;
			_contextAccessor = contextAccessor;
			_context = kiCdbContext;
		}

		[HttpGet]
		public IActionResult Register()
		{
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }

            RegisterViewModel rvm = new RegisterViewModel()
			{
				LegalName = "",
				EmailAddress = "",
				FetName = "",
				UserName = "",
				Password = "",
				Password2 = ""
			};
		
			return View(rvm);
		}
		
		[HttpPost]
		public IActionResult Register(RegisterViewModel rvmUpdated)
		{
			if (rvmUpdated.Password != rvmUpdated.Password2)
			{
				ViewBag.ErrorMessage = "Passwords do not match.";
				rvmUpdated.Password = "";
				rvmUpdated.Password2 = "";
				return View(rvmUpdated);
			}
		
			WebUser newUser = _userService.CreateUser(rvmUpdated);
		
			return View("~/Views/Member/RegisterSuccess.cshtml", newUser);
		}

		[HttpGet]
		[Route("~/Member/Login")]
		public IActionResult Login()
		{
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }

			UserViewModel uvm = new UserViewModel();

			return View(uvm);
        }

		[HttpPost]
		public IActionResult Login(UserViewModel uvmUpdated)
		{
			int? memberId = _context.User
				.Where(u => u.Username == uvmUpdated.UserName)
				.FirstOrDefault()
				.MemberId;

			if (memberId == null)
			{
				ViewBag.Error = "Username Incorrect.";
				return View(uvmUpdated);
			}

			string hash = HashPassword(uvmUpdated.Password, memberId);

			string compareHash = _context.User
				.Where(u => u.MemberId == memberId)
				.FirstOrDefault()
				.Password;

			if(compareHash != hash)
			{
				ViewBag.Error = "Password Incorrect.";
				return View(uvmUpdated);
			}

			string authToken = _context.User
				.Where(u => u.MemberId == memberId)
				.FirstOrDefault()
				.Token;

			if (authToken != null)
			{
				SetAuthToken(authToken, _contextAccessor.HttpContext);
				return Redirect("Admin/adminServices");
			}
			else
			{
				ViewBag.Error = "There was a problem, please contact your system administrator.";
				return View(uvmUpdated);
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}

		private string HashPassword(string password, int? memberId)
		{
			byte[] salt = Encoding.UTF8.GetBytes(memberId.ToString());
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

			byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
				passwordBytes,
				salt,
				350000,
				HashAlgorithmName.SHA512,
				64);

			return hash.ToString();
		}

		private void SetAuthToken(string authToken, HttpContext context)
		{
			_contextAccessor.HttpContext = context;

			CookieOptions cookieOptions = _cookieService.NewCookieFactory();

			context.Response.Cookies.Append("kic_auth", authToken, cookieOptions);
		}
	}
}
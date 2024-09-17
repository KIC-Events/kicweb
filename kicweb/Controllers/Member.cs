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
	public class Member : KICController
	{
		private readonly IConfigurationRoot _config;
		private readonly ILogger<Member> _logger;
		private readonly ICookieService _cookieService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IUserService _userService;
		private readonly KiCdbContext _context;

		public Member(IConfigurationRoot configurationRoot, ILogger<Member> logger, IUserService userService, ICookieService cookieService, IHttpContextAccessor contextAccessor, KiCdbContext kiCdbContext) : base(configurationRoot, userService: null, contextAccessor, kiCdbContext, cookieService)
		{
			_config = configurationRoot;
			_logger = logger;
			_userService = userService;
			_cookieService = cookieService;
			_contextAccessor = contextAccessor;
			_context = kiCdbContext;
		}

		[HttpGet]
		public IActionResult Register()
		{
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
            UserViewModel uvm = new UserViewModel();

			return View(uvm);
        }

		[HttpPost]
		[Route("~/Member/Login")]
		public IActionResult Login(UserViewModel uvmUpdated)
		{
			User? user;

			try
			{
				user = _context.User
				.Where(u => u.Username == uvmUpdated.UserName)
				.FirstOrDefault();
            }
			catch(NullReferenceException ex)
			{
				ViewBag.Error = "Username Incorrect.";
				return View(uvmUpdated);
			}
			catch(Exception ex)
			{
                ViewBag.Error = "There was a problem, please contact your system administrator.";
				return View(uvmUpdated);
            }

			if(user is null)
			{
                ViewBag.Error = "Username Incorrect.";
                return View(uvmUpdated);
            }

			string hash = HashPassword(uvmUpdated.Password, user.MemberId);

			string compareHash = user.Password;

			if(compareHash != hash)
			{
				ViewBag.Error = "Password Incorrect.";
				return View(uvmUpdated);
			}

			string? userName = user.Username;
			string? authToken = user.Token;

			if (authToken != null && userName != null)
			{
				SetAuthToken(userName, authToken, _contextAccessor.HttpContext);
				return Redirect("~/Admin/AdminServices");
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
			string salted = password + memberId.ToString();

			var bytes = Encoding.UTF8.GetBytes(salted);

			using(var hash = SHA512.Create())
			{
				var hashedBytes = hash.ComputeHash(bytes);

                var hashPW = BitConverter.ToString(hashedBytes).Replace("-", "");

                return hashPW;
			}
		}

		private void SetAuthToken(string userName, string authToken, HttpContext context)
		{
			_contextAccessor.HttpContext = context;

			CookieOptions cookieOptions = _cookieService.NewCookieFactory();

			context.Response.Cookies.Append("KICAuth", "true", cookieOptions);

			cookieOptions = _cookieService.NewCookieFactory();

			context.Response.Cookies.Append("UserName", userName, cookieOptions);

			cookieOptions = _cookieService.NewCookieFactory();

			context.Response.Cookies.Append("AuthToken", authToken, cookieOptions);
		}
	}
}
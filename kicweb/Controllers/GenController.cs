using KiCData.Models.WebModels;
using KiCData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using KiCData;
using KiCData.Services;

namespace KiCWeb.Controllers
{
    [Route("Gen")]
    public class GenController : KICController
    {
        private readonly IKiCLogger _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICookieService _cookieService;
        private readonly IEmailService _emailService;
        private readonly IConfigurationRoot _configurationRoot;
        private KiCdbContext _kdbContext;

        public GenController(IKiCLogger logger, IHttpContextAccessor httpContextAccessor, ICookieService cookieService, IEmailService emailService, IConfigurationRoot configurationRoot, KiCdbContext kiCdbContext, IUserService userService) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
        {
            _logger = logger;
            _contextAccessor = httpContextAccessor;
            _cookieService = cookieService;
            _emailService = emailService;
            _configurationRoot = configurationRoot;
            _kdbContext = kiCdbContext;
        }

        [Route("~/Privacy")]
        [Route("/Gen/Privacy")]
        public IActionResult Privacy()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
            }
            return View();
        }

        [Route("~/Club425")]
        [Route("/Gen/Club425")]
        public IActionResult Club425()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
            }
            return View();
        }

        [Route("~/About")]
        [Route("/Gen/About")]
        public IActionResult About()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
            }
            return View();
        }

        [Route("~/Purchase")]
        [Route("/Gen/Purchase")]
        public IActionResult Purchase()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
            }
            return View("/Views/Shared/UnderConstruction.cshtml");
        }

        [HttpGet]
        [Route("Volunteers/Register")]
        public IActionResult Volunteers()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
            }
            VolunteerViewModel volunteer = new VolunteerViewModel
            {
                Events = _kdbContext.Events.Where(a => a.StartDate > DateOnly.FromDateTime(DateTime.Now)).Select
                (a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList(),
                Positions = new List<SelectListItem>()
            {
            new SelectListItem(){ Value = "DM", Text ="Dungeon Monitor" },
            new SelectListItem(){ Value = "Door", Text =  "Door Person/Greeter" },
            new SelectListItem(){ Value = "Bar", Text = "Bartender" },
            new SelectListItem(){ Value = "Fire", Text = "Service Top - Fire" },
            new SelectListItem(){ Value = "Electric", Text = "Service Top - Electric" },
            new SelectListItem(){ Value = "Corporal", Text = "Service Top - Corporal" },
            new SelectListItem(){ Value = "Reg", Text = "Special Events - Registration" }
            },

            };

            return View(volunteer);
        }
        [HttpPost]
        [Route("Volunteers/Register")]
        public IActionResult Volunteers(VolunteerViewModel volUpdated)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "There was a validation issue.";
                return View(volUpdated);
            }
            else
            {
                KiCData.Models.Member member = new KiCData.Models.Member
                {
                    FirstName = volUpdated.FirstName,
                    LastName = volUpdated.LastName,
                    Email = volUpdated.Email,
                    DateOfBirth = volUpdated.DateOfBirth,
                    FetName = volUpdated.FetName,
                    ClubId = volUpdated.ClubId,
                    PhoneNumber = volUpdated.PhoneNumber
                };
                _kdbContext.Members.Add(member);
                _kdbContext.SaveChanges();
                List<string> list = new List<string>();
                foreach (var item in volUpdated.Positions)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }

                Volunteer volunteer = new Volunteer
                {
                    MemberId = member.Id,
                    Details = volUpdated.AdditionalInfo,
                    Positions = list
                };
                _kdbContext.Volunteers.Add(volunteer);
                _kdbContext.SaveChanges();
                if (volUpdated.EventId != 0)
                {
                    PendingVolunteer pendingVolunteer = new((int)volunteer.Id, volUpdated.EventId, volunteer.Positions.ToString());
                    _kdbContext.PendingVolunteers.Add(pendingVolunteer);
                    _kdbContext.SaveChanges();
                    return RedirectToAction("SubmissionSuccess", "People", new { v = "Volunteer" });
                }
            }

            return RedirectToAction("SubmissionSuccess", "People", new { v = "Volunteer" });
        }


        /*
        [HttpGet]
        public IActionResult Presenters()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
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
                return Redirect("~Home/Index");
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
                return Redirect("~Home/Index");
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
        */

        //https://github.com/Malechus/kic/issues/86
        /*
        [Route("~/Contact")]
        [Route("/Gen/Contact")]
        [HttpGet]
        public IActionResult Contact()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
            }
            ViewBag.Error = null;
            Feedback feedback = new Feedback();

            return View(feedback);
        }

        [Route("~/Contact")]
        [Route("/Gen/Contact")]
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
                _logger.Log(new Exception("The message is null."));

                return Redirect("Error");
            }

            message.HtmlBuilder.Append("<p>This is an automated email message sent through kicevents.com. A new feedback submission has occurred.</p>" +
                "<br />" +
                "<br />" +
                "<br /><b>Details: </b>" + feedbackUpdated.Text +
                "<br />" +
                "<br />"
            );

            if(feedbackUpdated.Name != null)
            {
                message.HtmlBuilder.AppendLine("<p><b>Sent by:</b> " + feedbackUpdated.Name + "</p>");
            }

            if(feedbackUpdated.Email != null)
            {
                message.HtmlBuilder.AppendLine("<p><b>Email for:</b> " + feedbackUpdated.Email + "</p>");
            }

            try
            {
                _emailService.SendEmail(message);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                return Redirect("Error");
            }

            return Redirect("Success");
        }
        */
        [Route("~/Contact")]
        [Route("/Gen/Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("~/GetInvolved")]
        [Route("/Gen/GetInvolved")]
        public IActionResult GetInvolved()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("~Home/Index");
            }
            return View();
        }

        [Route("~/Success")]
        [Route("/Gen/Success")]
        public IActionResult Success()
        {
            return View();
        }

        [Route("~/Error")]
        [Route("/Gen/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

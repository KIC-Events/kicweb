using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiCWeb.Controllers
{
    [Route("[controller]")]
    public class PeopleController : Controller
    {
        private readonly IKiCLogger _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICookieService _cookieService;
        private readonly IEmailService _emailService;
        private readonly IConfigurationRoot _configurationRoot;
        private KiCdbContext _kdbContext;

        public PeopleController(IKiCLogger logger, IHttpContextAccessor httpContextAccessor, ICookieService cookieService, IEmailService emailService, IConfigurationRoot configurationRoot, KiCdbContext kiCdbContext)
        {
            _logger = logger;
            _contextAccessor = httpContextAccessor;
            _cookieService = cookieService;
            _emailService = emailService;
            _configurationRoot = configurationRoot;
            _kdbContext = kiCdbContext;
        }
        public IActionResult Index()
        {
            if (!_cookieService.AgeGateCookieAccepted(_contextAccessor.HttpContext.Request))
            {
                return Redirect("Home/Index");
            }
            List<PendingVolunteer> pendings = _kdbContext.PendingVolunteers.Take(10).ToList();
            List<Volunteer> volunteers = _kdbContext.Volunteers.Take(10).ToList();
            List<KiCData.Models.Member> members = _kdbContext.Members.Take(10).ToList();
            List<Vendor> vendors = _kdbContext.Vendors.Take(10).ToList();
            List<Staff> staff = _kdbContext.Staff.Take(10).ToList();
            List<Presenter> presenters = _kdbContext.Presenters.Take(10).ToList();
            PeopleViewModel pVM = new PeopleViewModel(members, volunteers, pendings, vendors, staff, presenters);
            return View(pVM);
        }


        public IActionResult SubmissionSuccess(string personType)
        {
            ViewBag.PersonType = personType;
            return View();
        }
    }
}

using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HostFiltering;
using KiCData.Models.WebModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;


namespace KiCWeb.Controllers
{
	[Route("[controller]")]
	public class Admin : KICAuthController
	{

		private readonly IKiCLogger _logger;
		private readonly IConfigurationRoot _configurationRoot;
		private readonly IUserService _userService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly KiCdbContext _context;
		private readonly ICookieService _cookieService;

		public Admin(IKiCLogger logger, IConfigurationRoot configurationRoot, IUserService userService, IHttpContextAccessor httpContextAccessor, KiCdbContext kiCdbContext, ICookieService cookieService) : base(configurationRoot, userService, httpContextAccessor, kiCdbContext, cookieService)
		{
			_logger = logger;
			_userService = userService;
			_configurationRoot = configurationRoot;
			_context = kiCdbContext;
			_contextAccessor = httpContextAccessor;
			_cookieService = cookieService;
		}

		//Shows Basic Admin Tasks for Registration, Tickets, Vendors, Volunteers, and Presenters
		[HttpGet]
		[Route("~/Admin/AdminServices")]
		public IActionResult AdminServices()
		{
			return View();
		}

		[HttpGet]
		[Route("/Admin/Registration")]
		[Route("/Registration")]
		public IActionResult Registration()
		{
			CheckInViewModel cvm = new CheckInViewModel(_context);

			return View(cvm);
		}
		
		[HttpPost]		
		[Route("/Admin/Registration")]
		[Route("/Registration")]
		public IActionResult Registration(CheckInViewModel cvmUpdated)
		{
			try
			{
				cvmUpdated.Event = _context.Events
					.Where(e => e.Id == int.Parse(cvmUpdated.EventId))
					.FirstOrDefault();
			}
			catch(Exception ex)
			{
				_logger.Log(ex);
				return RedirectToAction("Registration");
			}
			
			try
			{
				cvmUpdated = OnPostSearch(cvmUpdated);
			}
			catch(Exception ex)
			{
				_logger.Log(ex);
				return RedirectToAction("Registration");
			}
			
			TempData["Checkin"] = JsonSerializer.Serialize<CheckInViewModel>(cvmUpdated);
			
			return RedirectToAction("Checkin");
		}

		[HttpGet]		
		[Route("/Admin/Checkin")]
		[Route("/Checkin")]		
		public IActionResult CheckIn()
		{
			CheckInViewModel cvmUpdated = JsonSerializer.Deserialize<CheckInViewModel>(TempData["Checkin"].ToString());
			
			if(cvmUpdated is null)
			{
				_logger.LogText("No data in checkin model.");
				return RedirectToAction("Registration");
			}

			return View(cvmUpdated);
		}
		
		[HttpPost]		
		[Route("/Admin/Checkin")]
		[Route("/Checkin")]		
		public IActionResult CheckIn(CheckInViewModel cvmUpdated)
		{
			Attendee attendee = _context.Attendees
				.Where(a => a.Id == int.Parse(cvmUpdated.ConfNumber))
				.First();
				
			attendee.isRegistered = true;
			
			_context.SaveChanges();
			
			return RedirectToAction("CheckInSuccess");
		}
		
		[Route("/Admin/CheckInSuccess")]
		[Route("/CheckInSuccess")]
		public IActionResult CheckInSuccess()
		{
			return View();
		}

		public CheckInViewModel OnPostSearch(CheckInViewModel cvmUpdated)
		{
			KiCData.Models.Member? member = null;

			try
			{
				member = MemberSearch(cvmUpdated);
			}
			catch(Exception ex)
			{
				_logger.Log(ex);
				throw ex;
			}

			if(member is null)
			{
				throw new RowNotInTableException();
			}

			Attendee? attendee = null;

			try
			{
				attendee = AttendeeSearch(member, cvmUpdated.Event);
			}
			catch(Exception ex)
			{
				_logger.Log(ex);
				throw ex;
			}
			
			if(attendee is null)
			{
				throw new RowNotInTableException();
			}

			cvmUpdated.FirstName = member.FirstName;
			cvmUpdated.LastName = member.LastName;
			cvmUpdated.City = member.City;
			cvmUpdated.State = member.State;
			cvmUpdated.DateOfBirth = member.DateOfBirth;
			cvmUpdated.ConfNumber = attendee.Id.ToString();
			cvmUpdated.Email = member.Email;
			cvmUpdated.MemberId = member.Id.ToString();
			cvmUpdated.BCComplete = attendee.BackgroundChecked;
			cvmUpdated.IsRegistered = attendee.isRegistered;
			cvmUpdated.IsPaid = attendee.IsPaid;

			return cvmUpdated;
		}

		public async Task<IActionResult> OnPostCheckinAsync(CheckInViewModel cvmUpdated)
		{
			if(cvmUpdated.IsEmpty())
			{
				_logger.LogText("Empty cvm on checkin execution.");
				return RedirectToAction("Registration");
			}

			if(!cvmUpdated.IsPaid)
			{
				//need to return with some way to alert that payment is not complete
				return RedirectToAction("Registration");
			}

			return RedirectToPage("Registration");
		}

		public KiCData.Models.Member? MemberSearch(CheckInViewModel cvmUpdated)
		{
			KiCData.Models.Member? member;
			if(cvmUpdated.FirstName is not null && cvmUpdated.LastName is not null)
			{
				try
				{
					member = _context.Members
						.Where(m => m.FirstName == cvmUpdated.FirstName && m.LastName == cvmUpdated.LastName)
						.FirstOrDefault();

					if(member is not null)
					{
						return member;
					}
				}
				catch(Exception ex)
				{
					_logger.Log(ex);
				}
			}

			if(cvmUpdated.FirstName is null || cvmUpdated.LastName is null)
			{
				try{
					Attendee attendee = _context.Attendees
						.Where(a => a.Id == int.Parse(cvmUpdated.ConfNumber))
						.First();
						
					member = _context.Members
						.Where(m => m.Id == attendee.MemberId)
						.FirstOrDefault();

					if(member is not null)
					{
						return member;
					}
					else
					{
						throw new Exception("Member not found.");
					}
				}
				catch(Exception ex)
				{
					_logger.Log(ex);
				}

				if(cvmUpdated.ConfNumber is null)
				{
					throw new Exception("Missing search criteria.");
				}
			}

			return null;
		}

		public Attendee AttendeeSearch(KiCData.Models.Member member, KiCData.Models.Event kicEvent)
		{
			Attendee? attendee = _context.Attendees
				.Where(a => a.MemberId == member.Id && a.Ticket.Event.Id == kicEvent.Id)
				.FirstOrDefault();

			if(attendee is not null)
			{
				return attendee;
			}
			else
			{
				throw new Exception("Attendee not found.");
			}
		}

		//Request for adding tickets to the database for an event
		[HttpGet]
		[Route("Tickets")]
		public IActionResult AdminTickets()
		{


			TicketViewModel ticket = new TicketViewModel
			{
				Events = _context.Events.Select(
					a => new SelectListItem 
					{ 
						Value = a.Id.ToString(),
						Text = a.Name
					}).ToList()
			};
			return View(ticket);
		}

		//Post request for adding tickets to the database for an event
		[HttpPost]
		public IActionResult AdminTickets(TicketViewModel ticket)
		{
			if (ModelState.IsValid)
			{
				for (int i = 0; i < ticket.QtyTickets; i++)
				{
					Ticket ticket1 = new Ticket
					{
						Price = ticket.Price,
						StartDate = ticket.StartDate,
						EndDate = ticket.EndDate,
						Type = ticket.Type
					};
					_context.Ticket.Add(ticket1);
					_context.SaveChanges();
					
				}
			}
			else
			{
				return View(ticket);
			}
			return RedirectToAction("Index");
		}

		//List of all events in the database
		[HttpGet]
		[Route("Events")]
		public IActionResult AdminEvents()
		{
			IEnumerable<Event> events = _context.Events.Include(b => b.Venue).ToList();
			return View(events);
		}

		//Details of a specific event
		[HttpGet]
		[Route("Events/{id}")]
		public IActionResult AdminEventDetails(int id)
		{
			Event events = _context.Events.Where(a => a.Id == id).FirstOrDefault();
			events.Venue = _context.Venue.Where(a => a.Id == events.VenueId).FirstOrDefault();
			events.Tickets = _context.Ticket.Where(a => a.EventId == id).ToList();
			
			return View(events);
		}

		//Request for adding a new event to the database
		[HttpGet]
		[Route("Events/Create")]
		public IActionResult AdminEventCreate()
		{
			EventViewModel evm = new EventViewModel
			{
				Venues = _context.Venue.Select(
					a => new SelectListItem
					{
						Value = a.Id.ToString(),
						Text = a.Name
					}).ToList()
			};

			return View(evm);
			
		}
		//Post request for adding a new event to the database
		[HttpPost]
		public IActionResult AdminEventCreate(EventViewModel model) 
		{
			if (ModelState.IsValid)
			{
				Event ev = new Event
				{
					Name = model.Name,
					StartDate = model.StartDate,
					EndDate = model.EndDate,
					Topic = model.Topic,
					Description = model.Description,
					VenueId = model.VenueId
				};
				_context.Events.Add(ev);
				_context.SaveChanges();
				return RedirectToAction("AdminEventDetails", new {ev.Id});
			}
			else
			{
				return View(model);
			}
		}

		//Request for editing an event in the database
		[HttpGet]
		[Route("Events/Edit/{id}")]
		public IActionResult AdminEventEdit(int id)
		{
			Event events = _context.Events.Where(a => a.Id == id).FirstOrDefault();
			
			return View(events);
		}
		/*
		 * Section for Volunteers, assigning pending volunteers to specific roles/shifts
		 * add volunteer to event, remove volunteer from event
		 */
		//Lists all volunteers that submitted for a specific event
		[HttpGet]
		[Route("Volunteers")]
		public IActionResult PendingVolunteerIndex()
		{
			IEnumerable<PendingVolunteer> pending = _context.PendingVolunteers.ToList();
			return View(pending);
		}

		//Approves and assigns a volunteer to a specific role/shift
		[HttpGet]
		[Route("Volunteers/Approve/{id}")]
		public IActionResult VolunteerApproval(int id)
		{
			PendingVolunteer pending = _context.PendingVolunteers.Where(a => a.Id == id).FirstOrDefault();
			if (pending == null)
			{
				return RedirectToAction("PendingVolunteerIndex");
			}
			else
			{
				//EventVolunteerViewModel eVVm = new EventVolunteerViewModel
				//{
				//    VolunteerId = pending.VolunteerID,
				//    VolunteerName = pending.Volunteer.Member.FetName,
				//    EventId = pending.EventId,
				//    EventName = pending.Event.Name,
				//    Positions = pending.PreferredPositions.Split(',').Select(a => new SelectListItem { Value = a, Text = a }).ToList()
				//};
			}
			
			return View(pending);
		}
		[HttpPost]
		public IActionResult VolunteerApproval(EventVolunteerViewModel eventVolunteer)
		{
			if (ModelState.IsValid)
			{
				EventVolunteer volunteer = new EventVolunteer
				{
					VolunteerId = eventVolunteer.VolunteerId,
					EventId = eventVolunteer.EventId,
					Position = eventVolunteer.Position,
					ShiftStart = eventVolunteer.ShiftStart,
					ShiftEnd = eventVolunteer.ShiftEnd
				};
				return RedirectToAction("PendingVolunteerIndex");
			}
			ViewBag.Error = "There is a problem validating this information. Please review";
			return View(eventVolunteer);
		}
		// end of volunteer section //


		/*
		 * Section for Vendors, Index, Details, Create, Edit, Delete
		 */
		[Route("Vendors")]
		public IActionResult VendorIndex()
		{
			IEnumerable<Vendor> vendors = _context.Vendors.ToList();
			return View(vendors);
		}

		public IActionResult VendorDetails(int id)
		{
			Vendor vendor = _context.Vendors.Where(a => a.Id == id).FirstOrDefault();
			if (vendor == null) 
			{
				ViewBag.Error = "Vendor not found";
				return RedirectToAction("VendorIndex");
			}
			return View(vendor);
		}


		// End of Vendor Section //
		//[HttpGet]
		//public IActionResult Login()
		//{
		//
		//}
	}
}

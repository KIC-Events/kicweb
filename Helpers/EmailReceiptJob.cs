using KiCData.Factories;
using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;

namespace KiCWeb.Helpers;

public class EmailReceiptJob
{
    private readonly KiCdbContext _db;
    private readonly IEmailService _emailService;
    
    public EmailReceiptJob(IEmailService emailService)
    {
        _emailService = emailService;
        var factory = new KiCdbContextFactory();
        _db = factory.CreateDbContext([]);
    }
    
    public string SendEmail(FormMessage message, int attendeeId)
    {
        _emailService.SendEmail(message);
        var attendee = _db.Attendees.First(x => x.Id == attendeeId);
        attendee.RegistrationConfirmationEmailTimestamp = DateTime.UtcNow;
        _db.Attendees.Update(attendee);
        _db.SaveChanges();
        return $"Sent email receipt for attendee id: {attendee.Id}";
    }
}
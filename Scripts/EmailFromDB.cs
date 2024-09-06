using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class EmailFromDB
    {
        private IConfigurationRoot _config;
        private KiCdbContext _dbContext;
        private IEmailService _emailService;

        public EmailFromDB(IConfigurationRoot configuration, KiCdbContext kiCdbContext, IEmailService emailService)
        {
            _config = configuration;
            _dbContext = kiCdbContext;
            _emailService = emailService;
        }

        public void SendEmailsFromDB()
        {
            List<TicketComp> ticketComps = _dbContext.TicketComp
                .Where(t => t.CompReason == "Club 425 Member Discount")
                .ToList();

            List<Mailer> mailers = new List<Mailer>();

            foreach (TicketComp ticketComp in ticketComps)
            {
                Attendee? attendee = _dbContext.Attendees
                    .Where(a => a.TicketId == ticketComp.TicketId)
                    .FirstOrDefault();
                if (attendee == null) { continue; }

                Member? member = _dbContext.Members
                    .Where(m => m.Id == attendee.MemberId)
                    .FirstOrDefault();

                if (member is not null)
                {
                    Console.WriteLine("Gathering data for " + member.FirstName + " " + member.LastName);
                    Mailer m = new Mailer(member.FirstName, member.LastName, member.Email);

                    m.CompAmt = ticketComp.CompAmount;
                    m.CompCode = ticketComp.DiscountCode;
                    
                    if(member.DateOfBirth == new DateOnly(1900, 01, 01) && m.Email.Contains('@'))
                    {
                        mailers.Add(m);
                    }
                }
            }

            Console.WriteLine("Generating email messages.");
            foreach (Mailer mailer in mailers)
            {

                Console.WriteLine("Generating email for " + mailer.FirstName + " " + mailer.LastName);

                FormMessage formMessage = new FormMessage();
                formMessage.To.Add(mailer.Email);
                formMessage.Subject = "Exciting News from Kinky In Columbus (KIC)";
                formMessage.HtmlBuilder.Append("<h1>Exciting News from Kinky In Columbus (KIC)</h1> " +
                        "<p> In case you were not at our August play party or saw the event posting that was put up shortly thereafter on Fet, Kinky In Columbus(KIC) will be hosting a hotel event in the Columbus Metropolitan area on January 10th & 11th, 2025.</p>"
                        + "<p>As a thank you for attending every party, some of the parties, or just signing up to be a Club 425 member, we are offering you a $" + mailer.CompAmt.ToString() + " discount off the purchase of a ticket just by putting in the code below at checkout. Please be aware that tickets and hotel rooms are sold on a first come, first served basis, so do not miss out on your opportunity to attend.</p>"
                        + "<p>We sincerely hope you will join us at our inaugural event titled<strong> Columbus’ Ultimate Relationship Exploratorium 2025 (CURE 2025)</strong>. This will be a great way to spend a weekend surrounded by like-minded individuals, learning new topics and skills from fantastic presenters, and upgrading your toy bag(s) with new wares from our awesome vendors.</p>"
                        + "<p><b>Enter this code on the CURE registration page before you enter your personal details, this code does not get entered on the payment page.</b></p>"
                        + "<p><b>" + mailer.CompCode + "</b></p>"
                        + "<p>We look forward to KIC’ing off the new year with you,</p>"
                        + "<p>Mike and Ashley<br> KIC Events</p>"
                    );

                mailer.FormMessage = formMessage;
            }
            Console.WriteLine("Email generation complete.");

            Console.WriteLine("Sending Emails");
            foreach(Mailer mailer in mailers)
            {
                Console.WriteLine(mailer.FirstName + " " + mailer.LastName);
                _emailService.SendEmail(mailer.FormMessage);
            }
        }
    }
}

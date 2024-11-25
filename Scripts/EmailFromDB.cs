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
                .Where(t => t.CompReason == "Scholarship")
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
                    
                    if(m.Email.Contains('@'))
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
                formMessage.Cc.Add("volunteers@kicevents.com");
                formMessage.Subject = "CURE Scholarship Opportunity";
                formMessage.HtmlBuilder.Append("<h1>CURE Scholarship Opportunity</h1> " +
                        "<p>You are receiving this email because you have signed up to volunteer for an extended shift at CURE on January 10th & 11th, 2025.</p>"
                        + "<p>In exchange for this shift, your registration for the event is being paid for. You will need to register as normal, but should not be charged for your ticket.</p>"
                        + "<p>If you have any issues, please contact technology@kicevents.com. </p>"
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

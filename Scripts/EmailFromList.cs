using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiCData.Models;
using KiCData.Services;
using IronXL;
using System.Net;
using KiCData.Models.WebModels;
using MimeKit.Encodings;
using Org.BouncyCastle.Crypto;
using Square.Models;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace Scripts
{
    public class EmailFromList
    {
        private List<Mailer> mailers = new List<Mailer>();
        public EmailFromList(string filePath)
        {
            Console.WriteLine("Accessing workbook...");
            WorkBook _workbook = new WorkBook(filePath);
            Console.WriteLine("Accessing default worksheet...");
            WorkSheet _worksheet = _workbook.WorkSheets.FirstOrDefault();
            Console.WriteLine("Consuming list...");
            foreach(RangeRow row in _worksheet.Rows)
            {
                string fName = row.Columns[0].ToString();
                string lName = row.Columns[1].ToString();
                string email = row.Columns[2].ToString();

                Mailer mailer = new Mailer(fName, lName, email);

                if (email is null) { break; }

                if (email.Contains('@'))
                {
                    Console.WriteLine("Adding record for " + fName + " " + lName);
                    mailers.Add(mailer);
                }
            }
        }

        public void GetCompCodes(KiCdbContext context)
        {
            Console.WriteLine("Retrieving Comp Codes...");
            foreach(Mailer mailer in mailers)
            {
                Console.WriteLine("Retrieving Comp Code for " + mailer.FirstName +  " " + mailer.LastName);
                Member member = context.Members
                    .Where(m => m.FirstName == mailer.FirstName && m.LastName == mailer.LastName)
                    .FirstOrDefault();
                Attendee attendee = context.Attendees
                    .Where(a => a.MemberId == member.Id)
                    .FirstOrDefault();
                Ticket ticket = context.Ticket
                    .Where(t => t.Id == attendee.TicketId)
                    .FirstOrDefault();
                TicketComp? comp = context.TicketComp
                    .Where(c => c.TicketId == ticket.Id)
                    .FirstOrDefault();

                string compCode;
                if (comp is not null) { compCode = comp.DiscountCode; }
                else { throw new KeyNotFoundException(); }
                Console.WriteLine("Comp Code is " + compCode);

                mailer.CompCode = compCode;
                mailer.CompAmt = comp.CompAmount;

                if(member.DateOfBirth != new DateOnly(1900, 01, 01))
                {
                    mailer.AlreadyRegistered = true;
                }
            }
            Console.WriteLine("Comp Code Retrieval Complete....");
        }

        public void BuildEmails()
        {
            Console.WriteLine("Generating email messages.");
            foreach(Mailer mailer in mailers)
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
        }

        public void SendEmails(IEmailService emailService)
        {
            Console.WriteLine("Sending emails...");
            foreach (Mailer mailer in mailers)
            {
                if(mailer.AlreadyRegistered != true)
                {
                    Console.WriteLine("Sending email to " + mailer.FirstName + " " + mailer.LastName);
                    emailService.SendEmail(mailer.FormMessage);
                }
            }
            Console.WriteLine("Sending emails complete...");
        }
    }

    public class Mailer
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string? CompCode { get; set; }
        public double? CompAmt { get; set; }
        public bool? AlreadyRegistered { get; set; }
        public FormMessage? FormMessage { get; set; }

        public Mailer(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}

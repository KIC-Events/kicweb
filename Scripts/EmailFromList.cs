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
                formMessage.Subject = "KIC EVENTS | A special discount for CURE 2025!";
                formMessage.HtmlBuilder.Append(
                        "<h3>A special discount code has been generated for you!</h3>" 
                        + "<h4><b>" + mailer.CompCode + "</b></h4>" 
                        + "<p>This code will get you $" + mailer.CompAmt.ToString() + " off your ticket purchase for CURE 2025.</p>"
                        + "<p>You are receiving this code early, please hold onto it, and more information will follow.</p>" 
                        + "<p>We look forward to seeing you at CURE!</p>"
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
                Console.WriteLine("Sending email to " + mailer.FirstName + " " + mailer.LastName);
                emailService.SendEmail(mailer.FormMessage);
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
        public FormMessage? FormMessage { get; set; }

        public Mailer(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}

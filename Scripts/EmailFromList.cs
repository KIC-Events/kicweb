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

                if (email.Contains('@'))
                {
                    Console.WriteLine("Adding record for " + fName + " " + lName);
                    mailers.Add(mailer);
                }
            }
        }

        public void GetCompCodes(KiCdbContext context)
        {
            Console.WriteLine("Generating Comp Codes...");
            int i = 0;
            foreach(Mailer mailer in mailers)
            {
                Console.WriteLine("Generating Comp Code for " + mailer.FirstName +  " " + mailer.LastName);
                string compCode = "KICVOL2025-" + i.ToString();
                Console.WriteLine("Comp Code is " + compCode);

                mailer.CompCode = compCode;

                TicketComp ticketComp = new TicketComp()
                {
                    DiscountCode = compCode,
                    CompReason = "2025 Volunteer",
                    AuthorizingUser = "System"
                };

                Console.WriteLine("Adding comp to database...");
                context.TicketComp.Add(ticketComp);
                context.SaveChanges();
            }
            Console.WriteLine("Comp Code Generation Complete....");
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
                        + "<p>This code will get you $25 off your ticket purchase for CURE 2025.</p>"
                        + "<p>We wanted to thank you for your help in building our community.</p>" 
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
        public FormMessage? FormMessage { get; set; }

        public Mailer(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}

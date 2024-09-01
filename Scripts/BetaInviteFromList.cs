using IronXL;
using KiCData.Models.WebModels;
using KiCData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class BetaInviteFromList
    {
        private List<Mailer> mailers = new List<Mailer>();
        public BetaInviteFromList(string filePath)
        {
            Console.WriteLine("Accessing workbook...");
            WorkBook _workbook = new WorkBook(filePath);
            Console.WriteLine("Accessing default worksheet...");
            WorkSheet _worksheet = _workbook.WorkSheets.FirstOrDefault();
            Console.WriteLine("Consuming list...");
            foreach (RangeRow row in _worksheet.Rows)
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

        public void BuildEmails()
        {
            Console.WriteLine("Generating email messages.");
            foreach (Mailer mailer in mailers)
            {
                Console.WriteLine("Generating email for " + mailer.FirstName + " " + mailer.LastName);

                FormMessage formMessage = new FormMessage();
                formMessage.To.Add(mailer.Email);
                formMessage.Subject = "KIC EVENTS | Early Registration Access!";
                formMessage.HtmlBuilder.Append(
                        "<h3>You have been invited to register early for CURE as part of a closed beta test!</h3>"
                        + "<p>The purpose of this test is to make sure our registration system is working completely as expected before we go public. In exchange for your help with this testing, you will be able to purchase your ticket before anyone else, guaranteeing you a ticket to the event and a hotel room, if you want it.</p>"
                        + "<p>This is a live test, using real money, not a sandbox environment. Sandbox testing will have already concluded before the beta begins.</p>" 
                        + "<p>This is a closed beta, please do not share this information with anyone.</p>"
                        + "<br />" 
                        + "<p>The beta test will take place on Monday, September 2nd 2024 at 8pm EST. It will end after 24 hours.</p>"
                        + "<p>To participate, please <a href=\"https://discord.gg/ZvbvqMB9sJ\">click here</a> to join the beta test Discord server.</p> "
                        + "<p>If you are not able to join the server, you can email technology@kicevents.com for access as well, but support via email may be limited.</p>"
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
}

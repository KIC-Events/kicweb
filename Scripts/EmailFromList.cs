using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiCData.Models;
using KiCData.Services;
using IronXL;
using System.Net;

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

                mailers.Add(mailer);
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
                    Id = compCode,
                    CompReason = "KIC Volunteer",
                    AuthorizingUser = "System"
                };

                Console.WriteLine("Adding comp to database...");
                context.TicketComp.Add(ticketComp);
                context.SaveChanges();
            }
            Console.WriteLine("Comp Code Generation Complete....");
        }
    }

    public class Mailer
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string? CompCode { get; set; }

        public Mailer(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
}

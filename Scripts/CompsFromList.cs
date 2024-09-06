using IronXL;
using KiCData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class CompsFromList
    {
        private List<TicketComp> comps = new List<TicketComp>();

        public CompsFromList(string filePath, KiCdbContext kiCdbContext)
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
                string fetName = row.Columns[3].ToString();
                string compReason = row.Columns[5].ToString();
                string authUser = row.Columns[6].ToString();

                Console.WriteLine("Processing " + fName + " " + lName);

                Console.WriteLine("Creating ticket comp.");
                TicketComp comp = new TicketComp();
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[15];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                comp.DiscountCode = new String(stringChars);

                comp.CompAmount = 160;
                comp.CompReason = compReason;
                comp.AuthorizingUser = authUser;
                comp.CompPct = 100;

                Event CURE = kiCdbContext.Events
                    .Where(e => e.Name.Contains("CURE"))
                    .First();

                comp.Ticket = new Ticket()
                {
                    EventId = CURE.Id,
                    Event = CURE,
                    Price = 180,
                    Type = "Comp",
                    IsComped = true,
                    Attendee = new Attendee()
                    {
                        IsPaid = false,
                        isRegistered = false,
                        RoomWaitListed = true,
                        TicketWaitListed = true,
                        BadgeName = "TBD",
                        Member = new Member()
                        {
                            FirstName = fName,
                            LastName = lName,
                            Email = email,
                            DateOfBirth = new DateOnly(1900, 01, 01),
                            FetName = fetName
                        }
                    }
                };

                comps.Add(comp);
            }
        }

        public void WriteToDB(KiCdbContext kiCdbContext)
        {
            Console.WriteLine("Saving comps to database");
            foreach(TicketComp comp in comps)
            {
                Console.WriteLine("Creating records for " + comp.Ticket.Attendee.Member.LegalName);
                Console.WriteLine("Creating member record.");
                kiCdbContext.Members.Add(comp.Ticket.Attendee.Member);
                Console.WriteLine("Creating attendee record.");
                kiCdbContext.Attendees.Add(comp.Ticket.Attendee);
                Console.WriteLine("Creating ticket record.");
                kiCdbContext.Ticket.Add(comp.Ticket);
                Console.WriteLine("Creating ticket comp.");
                kiCdbContext.TicketComp.Add(comp);
            }
            kiCdbContext.SaveChanges();
        }
    }
}

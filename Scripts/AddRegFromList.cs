using IronXL;
using KiCData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class AddRegFromList
    {
        private KiCdbContext _context;
        public AddRegFromList(KiCdbContext kiCdbContext)
        {
            _context = kiCdbContext;
        }

        public void ProcessList(string filePath)
        {
            Console.WriteLine("Accessing workbook...");
            WorkBook _workbook = new WorkBook(filePath);
            Console.WriteLine("Accessing default worksheet...");
            WorkSheet _worksheet = _workbook.WorkSheets.FirstOrDefault();
            Console.WriteLine("Consuming list...");
            for(int i = 1; i >= 1 && i <= 161; i++)
            {
                RangeRow row = _worksheet.Rows[i];
                Member? m = _context.Members
                    .Where(m => m.Id == int.Parse(row.Columns[0].ToString()))
                    .FirstOrDefault();

                if (m is null)
                {
                    int? t = _context.Ticket.OrderBy(t => t.Id).Last().Id;
                    Member member = new Member();
                    Attendee attendee = new Attendee();
                    Ticket ticket = new Ticket();

                    member.Id = int.Parse(row.Columns[0].ToString());
                    attendee.MemberId = int.Parse(row.Columns[0].ToString());
                    attendee.ConfirmationNumber = int.Parse(row.Columns[1].ToString());
                    attendee.Id = int.Parse(row.Columns[1].ToString());
                    member.FirstName = row.Columns[2].ToString();
                    member.LastName = row.Columns[3].ToString();
                    member.Email = row.Columns[4].ToString();
                    member.DateOfBirth = DateOnly.FromDateTime(DateTime.Parse(row.Columns[6].ToString()));
                    member.SexOnID = row.Columns[7] is not null ? row.Columns[7].ToString() : string.Empty;
                    member.City = row.Columns[8] is not null ? row.Columns[8].ToString() : string.Empty;
                    member.State = row.Columns[9] is not null ? row.Columns[9].ToString() : string.Empty;
                    attendee.RoomPreference = row.Columns[13] is not null ? row.Columns[13].ToString() : string.Empty;
                    attendee.Pronouns = row.Columns[14] is not null ? row.Columns[14].ToString() : string.Empty;
                    attendee.BadgeName = row.Columns[15] is not null ? row.Columns[15].ToString() : string.Empty;
                    member.FetName = row.Columns[16] is not null ? row.Columns[16].ToString() : string.Empty;
                    ticket.DatePurchased = DateOnly.FromDateTime(DateTime.Parse(row.Columns[17].ToString()));
                    ticket.EventId = 1112;
                    ticket.Id = t + 1;
                    attendee.TicketId = t + 1;

                    attendee.Member = member;
                    attendee.Ticket = ticket;

                    _context.Members.Add(member);
                    _context.Attendees.Add(attendee);
                    _context.Ticket.Add(ticket);
                    _context.SaveChanges();

                    Console.WriteLine("Added " + member.FirstName + " " + member.LastName);
                }
                else
                {
                    Console.WriteLine(m.FirstName + " " + m.LastName + " Exists");
                }
            }
        }
    }
}

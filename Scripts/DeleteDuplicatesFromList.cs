using IronXL;
using KiCData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class DeleteDuplicatesFromList
    {
        private KiCdbContext _context;
        public DeleteDuplicatesFromList(KiCdbContext kiCdbContext)
        {
            _context = kiCdbContext;
        }

        public void RunDelete(string filePath)
        {
            Console.WriteLine("Accessing workbook...");
            WorkBook _workbook = new WorkBook(filePath);
            Console.WriteLine("Accessing default worksheet...");
            WorkSheet _worksheet = _workbook.WorkSheets.FirstOrDefault();
            Console.WriteLine("Consuming list...");
            foreach (RangeRow row in _worksheet.Rows)
            {
                int memberID = int.Parse(row.Columns[0].ToString());
                int attendeeID = int.Parse(row.Columns[1].ToString());
                int ticketID = int.Parse(row.Columns[2].ToString());

                Attendee? attendee = _context.Attendees
                    .Where(a => a.Id == attendeeID)
                    .FirstOrDefault();

                if(attendee != null)
                {
                    Console.WriteLine("Deleting Attendee " + attendeeID);
                    _context.Attendees.Remove(attendee);
                }

                TicketComp? ticketComp = _context.TicketComp
                    .Where(tc => tc.TicketId == ticketID)
                    .FirstOrDefault();

                if(ticketComp != null)
                {
                    Console.WriteLine("Deleting ticket comp");
                    _context.TicketComp.Remove(ticketComp);
                }

                Ticket? ticket = _context.Ticket
                    .Where(t => t.Id == ticketID)
                    .FirstOrDefault();

                if(ticket != null)
                {
                    Console.WriteLine("Deleting ticket " + ticketID);
                    _context.Ticket.Remove(ticket);
                }

                Member? member = _context.Members
                    .Where(m => m.Id == memberID)
                    .FirstOrDefault();

                if(member != null)
                {
                    Console.WriteLine("Deleting Member " + memberID);
                    _context.Members.Remove(member);
                }
            }

            _context.SaveChanges();
        }
    }
}

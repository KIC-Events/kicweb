using KiCData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class DeleteDuplicatesFromDB
    {
        private KiCdbContext _context;
        public DeleteDuplicatesFromDB(KiCdbContext kiCdbContext)
        {
            _context = kiCdbContext;
        }

        public void RunDelete()
        {
            List<Member> members = _context.Members.ToList();
            List<Attendee> attendees = _context.Attendees.ToList();
            List<Ticket> tickets = _context.Ticket.ToList();
            List<TicketComp> ticketComps = _context.TicketComp.ToList();

            foreach (Member member in members)
            {
                foreach(Member m in members)
                {
                    if(member.FirstName == m.FirstName && member.LastName == m.LastName)
                    {
                        if(member.Id != m.Id)
                        {
                            Attendee? at = attendees
                                .Where(a => a.MemberId == m.Id)
                                .FirstOrDefault();

                            if (at != null) 
                            {
                                Console.WriteLine("Deleting Attendee " + at.Id.ToString());
                                _context.Attendees.Remove(at);

                                Ticket? ti = tickets
                                    .Where(t => t.Id == at.TicketId)
                                    .FirstOrDefault();

                                if(ti != null)
                                {
                                    Console.WriteLine("Deleting Ticket " + ti.Id.ToString());
                                    _context.Ticket.Remove(ti);

                                    TicketComp? tc = ticketComps
                                        .Where(t => t.TicketId == ti.Id)
                                        .FirstOrDefault();

                                    if (tc != null) 
                                    { 
                                        Console.WriteLine("Deleting TicketComp " + tc.Id.ToString());
                                        _context.TicketComp.Remove(tc); 
                                    }
                                }
                            }

                            Console.WriteLine("Deleting Member " + m.Id.ToString());

                            _context.Members.Remove(m);
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}

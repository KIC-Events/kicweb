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
        private List<TicketComp> comps;

        public CompsFromList(string filePath)
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
                string status = row.Columns[4].ToString();
                string compReason = row.Columns[5].ToString();

                TicketComp comp = new TicketComp();
                if(status == "Comp Code")
                {
                    comp.CompPct = 100;
                    comp.Ticket = new Ticket()
                    {
                        EventId = 1112,
                        Price = 180,
                        Type = "Comp",
                        IsComped = true,
                        Attendee = new Attendee()
                        {
                            IsPaid = false,
                            isRegistered = true,
                            RoomWaitListed = true,
                            TicketWaitListed = true,
                            Member = new Member()
                            {
                                FirstName = fName,
                                LastName = lName,
                                Email = email,
                                FetName = fetName
                            }
                        }
                    };

                    comps.Add(comp);
                }
            }
        }


    }
}

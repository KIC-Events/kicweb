using System;
using KiCData.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using KiCData.Services;
using System.Text.Json;

namespace Scripts;

public class CompsFromJson
{
    private KiCdbContext _context;
    private List<CompObj> compObjs;

    public CompsFromJson(string filePath, KiCdbContext context)
    {
        _context = context;

        if(!File.Exists(filePath))
        {
            Console.WriteLine("File not found.");
            Environment.Exit(1);
        }

        string jsonValue;

        using(StreamReader sr = new StreamReader(File.ReadAllText(filePath)))
        {
            jsonValue = sr.ToString();
        }

        compObjs = JsonSerializer.Deserialize<List<CompObj>>(jsonValue);
    }

    public void JsonToModel()
    {
        foreach(CompObj obj in compObjs)
        {
            Console.WriteLine("Working on entry for " + obj.FirstName + " " + obj.LastName);

            Attendee attendee = new Attendee();
            Ticket ticket = new Ticket();

            Console.WriteLine("Checking for existing member in DB.");

            Member member = _context.Members
                .Where(m => m.FirstName == obj.FirstName
                && m.LastName == obj.LastName)
                .FirstOrDefault();

            if(member is null){
                Console.WriteLine("No entry found, creating new entry.");

                member = new Member(){
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    Email = obj.Email,
                    FetName = obj.FetName
                };
            }

            Console.WriteLine("Creating attendee model.");
            attendee.Ticket = ticket;
            attendee.Member = member;
            attendee.BackgroundChecked = false;
            attendee.IsPaid = true;

            Console.WriteLine("Creating ticket.");
            ticket.Attendee = attendee;
            ticket.Event = _context.Events
                .Where(e => e.Name == "CURE")
                .First();
            ticket.IsComped = true;
            ticket.Price = 0;

            Console.WriteLine("Creating comp.");
            char[] chars = obj.FetName.ToCharArray();

            TicketComp ticketComp = new TicketComp(){
                DiscountCode = "SCHOL" + chars[0] + chars[1] + chars[2],
                CompAmount = 180,
                CompPct = 100,
                CompReason = obj.CompedCategory,
                Ticket = ticket
            };

            Console.WriteLine("Writing data to DB.");
            _context.Members.Add(member);
            _context.Attendees.Add(attendee);
            _context.Ticket.Add(ticket);
            _context.TicketComp.Add(ticketComp);
            _context.SaveChanges();
        }
    }
}

public class CompObj
{
    public string FirstName { get;set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string FetName { get; set; }

    public string TicketType { get; set; }

    public string CompedCategory { get; set; }

    public string ApprovedBy { get; set; }
}
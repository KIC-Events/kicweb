using System.Threading.Tasks;
using Hangfire;
using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;

namespace KiCWeb.Helpers;

public static class CureRegistrationHelpers
{
    public static Attendee CreateAttendeeFromRegistration(KiCdbContext ctx, RegistrationViewModel reg, int eventId)
    {
        var member = ctx.Members.Add(
            new Member
            {
                FirstName = reg.FirstName,
                LastName = reg.LastName,
                Email = reg.Email,
                DateOfBirth = reg.DateOfBirth,
                FetName = reg.FetName,
                ClubId = reg.ClubId,
                PhoneNumber = reg.PhoneNumber,
                SexOnID = reg.SexOnID,
                City = reg.City,
                State = reg.State
            });
        var ticket = ctx.Ticket.Add(new Ticket
        {
            EventId = eventId
        });
        ctx.SaveChanges();
        var attendee = ctx.Attendees.Add(
            new Attendee
            {
                BadgeName = reg.BadgeName,
                Pronouns = reg.Pronouns,
                BackgroundChecked = false,
                RoomPreference = reg.RoomType,
                IsPaid = false,
                IsRefunded = false,
                MemberId = member.Entity.Id,
                TicketId = ticket.Entity.Id,
            });
        ctx.SaveChanges();
        return attendee.Entity;
    }

    public static string? FinalizeTicketOrder(InventoryService inventoryService, PaymentService paymentService,
        List<RegistrationViewModel> registrationViewModels, List<Attendee> attendees)
    {
        paymentService.SetAttendeesPaid(attendees);
        inventoryService.AdjustInventory(registrationViewModels);

        return paymentService.getOrderID(registrationViewModels);
    }
    
    public static void UpdateOrderID(KiCdbContext ctx, List<Attendee> attendees, string orderId)
    {
        foreach(Attendee attendee in attendees)
        {
            attendee.OrderID = orderId;
            ctx.Attendees.Update(attendee);
        }

        ctx.SaveChanges();
    }
    
    public static void WriteNonPaymentTickets(KiCdbContext ctx, List<RegistrationViewModel> registrationViewModels, string orderId)
    {
        foreach(RegistrationViewModel rvm in registrationViewModels)
        {
            TicketComp tc = ctx.TicketComp
                .Where(t => t.Id == rvm.TicketComp.Id)
                .First();

            Ticket ticket = ctx.Ticket
                .Where(t => t.Id == tc.TicketId)
                .First();

            Attendee attendee = ctx.Attendees
                .Where(a => a.TicketId == ticket.Id)
                .First();

            Member member = ctx.Members
                .Where(m => m.Id == attendee.MemberId)
                .First();

            attendee.BadgeName = rvm.BadgeName;
            attendee.isRegistered = true;
            attendee.OrderID = orderId;
            attendee.Pronouns = rvm.Pronouns;
            attendee.RoomPreference = rvm.RoomType;

            ticket.DatePurchased = DateOnly.FromDateTime(DateTime.Today);
            ticket.Type = rvm.TicketType;
            ticket.HasMealAddon = rvm.HasMealAddon;

            member.City = rvm.City;
            member.DateOfBirth = rvm.DateOfBirth;
            member.Email = rvm.Email;
            member.FetName = rvm.FetName;
            member.ClubId = rvm.ClubId;
            member.LastName = rvm.LastName;
            member.PhoneNumber = rvm.PhoneNumber;
            member.SexOnID = rvm.SexOnID;
            member.State = rvm.State;
            member.FirstName = rvm.FirstName;

            ctx.Update(ticket);
            ctx.Update(attendee);
            ctx.Update(member);
        }

        ctx.SaveChanges();
    }
}
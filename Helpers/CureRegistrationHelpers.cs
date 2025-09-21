using System.Threading.Tasks;
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

    public static async Task<string?> FinalizeTicketOrder(InventoryService inventoryService, PaymentService paymentService,
        List<RegistrationViewModel> registrationViewModels, List<Attendee> attendees)
    {
        paymentService.SetAttendeesPaid(attendees);
        await inventoryService.AdjustInventoryAsync(registrationViewModels);

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
}
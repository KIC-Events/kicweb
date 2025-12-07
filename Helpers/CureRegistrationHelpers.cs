using System.Threading.Tasks;
using Hangfire;
using KiCData.Models;
using KiCData.Models.WebModels;
using KiCData.Services;
using KiCWeb.Models;
using Microsoft.AspNetCore.Mvc;

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
        //ScheduleEmailReceipt(attendees, backgroundJobClient, ctx, logger, config, emailService, paymentService);

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

    public static void WriteNonPaymentTickets(KiCdbContext ctx, List<RegistrationViewModel> registrationViewModels,
        string orderId)
    {
        foreach (RegistrationViewModel rvm in registrationViewModels)
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


    public static void ScheduleEmailReceipt(List<Attendee> attendees, IBackgroundJobClient bgClient, KiCdbContext ctx, ILogger logger, IConfigurationRoot config, PaymentService paymentService, ControllerContext controllerContext)
    {
        var order = ctx.Orders.First(x => x.SquareOrderId == attendees.First().OrderID);
        var paymentMethod = "Comp Ticket";
        if (order.PaymentsTotal > 0 && order.SquareOrderId != null)
        {
                paymentMethod = paymentService.GetPaymentMethodForOrder(order.SquareOrderId) ?? "Unknown";
        }

        var receiptViewModel = new ReceiptViewModel
        {
            Attendees = attendees,
            Order = order,
            PaymentMethod = paymentMethod
        };
        
        var renderedHtmlTemplate = ViewHelpers.RenderViewToStringAsync("receipt", receiptViewModel, controllerContext).Result;
        var renderedPlaintextTemplate = ViewHelpers.RenderViewToStringAsync("plaintextReceipt", receiptViewModel, controllerContext).Result;
        if (config["Email Addresses:From"] == null)
        {
            logger.LogError("Unable to generate email receipt. From address is not set in config. Set 'Email Addresses:From' to a valid email address to enable sending.");
            return;
        }
        foreach (var attendee in attendees)
        {
            if (attendee.Id == null)
            {
                logger.LogError($"Unable to generate email receipt: Attendee {attendee.BadgeName} does not have a record id");
                continue;
            }
            if (attendee.Member == null)
            {
                logger.LogError($"Unable to generate email receipt: Attendee {attendee.Id} has no Member");
                continue;
            }
            if (attendee.Member.Email == null)
            {
                logger.LogError($"Unable to generate email receipt: Attendee {attendee.Id} has no email address (Member {attendee.Member.Id}).");
                continue;
            }

            var msg = new FormMessage
            {
                To = [attendee.Member.Email],
                Subject = "CURE 2026 Registration Receipt",
                From = config["Email Addresses:From"]!,
                Html = renderedHtmlTemplate,
                Text = renderedPlaintextTemplate,
            };
            var attendeeId = attendee.Id ?? 0;
            bgClient.Enqueue<EmailReceiptJob>((job) => job.SendEmail(msg, attendeeId));
        }
    }

    public static void SendEmail(FormMessage message, int attendeeId, KiCdbContext ctx, IEmailService emailService)
    {
        emailService.SendEmail(message);
        var attendee = ctx.Attendees.First(x => x.Id == attendeeId);
        attendee.RegistrationConfirmationEmailTimestamp = DateTime.UtcNow;
        ctx.Attendees.Update(attendee);
        ctx.SaveChanges();
    }
}
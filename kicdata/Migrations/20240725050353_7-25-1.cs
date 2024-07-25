using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class _7251 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendees_Members_MemberId",
                table: "Attendees");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendees_Ticket_TicketId",
                table: "Attendees");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venue_VenueId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVendors_Events_EventId",
                table: "EventVendors");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVendors_Vendors_VendorId",
                table: "EventVendors");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVolunteers_Events_EventId",
                table: "EventVolunteers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVolunteers_Volunteers_VolunteerId",
                table: "EventVolunteers");

            migrationBuilder.DropForeignKey(
                name: "FK_Presentations_Events_EventId",
                table: "Presentations");

            migrationBuilder.DropForeignKey(
                name: "FK_Presentations_Presenters_PresenterId",
                table: "Presentations");

            migrationBuilder.DropForeignKey(
                name: "FK_Presenters_Members_MemberId",
                table: "Presenters");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Members_MemberId",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Events_EventId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Members_MemberId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Members_MemberId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Members_MemberId",
                table: "Volunteers");

            migrationBuilder.DropForeignKey(
                name: "FK_WaitList_Attendees_AttendeeId",
                table: "WaitList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Volunteers",
                table: "Volunteers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendors",
                table: "Vendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Presenters",
                table: "Presenters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Presentations",
                table: "Presentations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventVolunteers",
                table: "EventVolunteers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventVendors",
                table: "EventVendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubMembers",
                table: "ClubMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendees",
                table: "Attendees");

            migrationBuilder.RenameTable(
                name: "Volunteers",
                newName: "Volunteer");

            migrationBuilder.RenameTable(
                name: "Vendors",
                newName: "Vendor");

            migrationBuilder.RenameTable(
                name: "Presenters",
                newName: "Presenter");

            migrationBuilder.RenameTable(
                name: "Presentations",
                newName: "Presentation");

            migrationBuilder.RenameTable(
                name: "Members",
                newName: "Member");

            migrationBuilder.RenameTable(
                name: "EventVolunteers",
                newName: "EventVolunteer");

            migrationBuilder.RenameTable(
                name: "EventVendors",
                newName: "EventVendor");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "ClubMembers",
                newName: "ClubMember");

            migrationBuilder.RenameTable(
                name: "Attendees",
                newName: "Attendee");

            migrationBuilder.RenameIndex(
                name: "IX_Volunteers_MemberId",
                table: "Volunteer",
                newName: "IX_Volunteer_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendors_MemberId",
                table: "Vendor",
                newName: "IX_Vendor_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Presenters_MemberId",
                table: "Presenter",
                newName: "IX_Presenter_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Presentations_PresenterId",
                table: "Presentation",
                newName: "IX_Presentation_PresenterId");

            migrationBuilder.RenameIndex(
                name: "IX_Presentations_EventId",
                table: "Presentation",
                newName: "IX_Presentation_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVolunteers_VolunteerId",
                table: "EventVolunteer",
                newName: "IX_EventVolunteer_VolunteerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVolunteers_EventId",
                table: "EventVolunteer",
                newName: "IX_EventVolunteer_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVendors_VendorId",
                table: "EventVendor",
                newName: "IX_EventVendor_VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVendors_EventId",
                table: "EventVendor",
                newName: "IX_EventVendor_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_VenueId",
                table: "Event",
                newName: "IX_Event_VenueId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendees_TicketId",
                table: "Attendee",
                newName: "IX_Attendee_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendees_MemberId",
                table: "Attendee",
                newName: "IX_Attendee_MemberId");

            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                table: "Vendor",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                table: "Presenter",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                table: "Presentation",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Volunteer",
                table: "Volunteer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendor",
                table: "Vendor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Presenter",
                table: "Presenter",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Presentation",
                table: "Presentation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Member",
                table: "Member",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventVolunteer",
                table: "EventVolunteer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventVendor",
                table: "EventVendor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClubMember",
                table: "ClubMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendee",
                table: "Attendee",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Presentation",
                keyColumn: "Id",
                keyValue: 2222,
                column: "ImgPath",
                value: "/wwwroot/Presentations/image01.jpg");

            migrationBuilder.UpdateData(
                table: "Presenter",
                keyColumn: "Id",
                keyValue: 1234,
                column: "ImgPath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Vendor",
                keyColumn: "Id",
                keyValue: 1128,
                column: "ImgPath",
                value: "/wwwroot/images/Vendors/image01.jpg");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendee_Member_MemberId",
                table: "Attendee",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendee_Ticket_TicketId",
                table: "Attendee",
                column: "TicketId",
                principalTable: "Ticket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_VenueId",
                table: "Event",
                column: "VenueId",
                principalTable: "Venue",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventVendor_Event_EventId",
                table: "EventVendor",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventVendor_Vendor_VendorId",
                table: "EventVendor",
                column: "VendorId",
                principalTable: "Vendor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventVolunteer_Event_EventId",
                table: "EventVolunteer",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventVolunteer_Volunteer_VolunteerId",
                table: "EventVolunteer",
                column: "VolunteerId",
                principalTable: "Volunteer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Presentation_Event_EventId",
                table: "Presentation",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Presentation_Presenter_PresenterId",
                table: "Presentation",
                column: "PresenterId",
                principalTable: "Presenter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Presenter_Member_MemberId",
                table: "Presenter",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Member_MemberId",
                table: "Staff",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Event_EventId",
                table: "Ticket",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Member_MemberId",
                table: "User",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendor_Member_MemberId",
                table: "Vendor",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteer_Member_MemberId",
                table: "Volunteer",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitList_Attendee_AttendeeId",
                table: "WaitList",
                column: "AttendeeId",
                principalTable: "Attendee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendee_Member_MemberId",
                table: "Attendee");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendee_Ticket_TicketId",
                table: "Attendee");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_VenueId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVendor_Event_EventId",
                table: "EventVendor");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVendor_Vendor_VendorId",
                table: "EventVendor");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVolunteer_Event_EventId",
                table: "EventVolunteer");

            migrationBuilder.DropForeignKey(
                name: "FK_EventVolunteer_Volunteer_VolunteerId",
                table: "EventVolunteer");

            migrationBuilder.DropForeignKey(
                name: "FK_Presentation_Event_EventId",
                table: "Presentation");

            migrationBuilder.DropForeignKey(
                name: "FK_Presentation_Presenter_PresenterId",
                table: "Presentation");

            migrationBuilder.DropForeignKey(
                name: "FK_Presenter_Member_MemberId",
                table: "Presenter");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Member_MemberId",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Event_EventId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Member_MemberId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendor_Member_MemberId",
                table: "Vendor");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteer_Member_MemberId",
                table: "Volunteer");

            migrationBuilder.DropForeignKey(
                name: "FK_WaitList_Attendee_AttendeeId",
                table: "WaitList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Volunteer",
                table: "Volunteer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendor",
                table: "Vendor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Presenter",
                table: "Presenter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Presentation",
                table: "Presentation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Member",
                table: "Member");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventVolunteer",
                table: "EventVolunteer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventVendor",
                table: "EventVendor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubMember",
                table: "ClubMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendee",
                table: "Attendee");

            migrationBuilder.DropColumn(
                name: "ImgPath",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "ImgPath",
                table: "Presenter");

            migrationBuilder.DropColumn(
                name: "ImgPath",
                table: "Presentation");

            migrationBuilder.RenameTable(
                name: "Volunteer",
                newName: "Volunteers");

            migrationBuilder.RenameTable(
                name: "Vendor",
                newName: "Vendors");

            migrationBuilder.RenameTable(
                name: "Presenter",
                newName: "Presenters");

            migrationBuilder.RenameTable(
                name: "Presentation",
                newName: "Presentations");

            migrationBuilder.RenameTable(
                name: "Member",
                newName: "Members");

            migrationBuilder.RenameTable(
                name: "EventVolunteer",
                newName: "EventVolunteers");

            migrationBuilder.RenameTable(
                name: "EventVendor",
                newName: "EventVendors");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "ClubMember",
                newName: "ClubMembers");

            migrationBuilder.RenameTable(
                name: "Attendee",
                newName: "Attendees");

            migrationBuilder.RenameIndex(
                name: "IX_Volunteer_MemberId",
                table: "Volunteers",
                newName: "IX_Volunteers_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendor_MemberId",
                table: "Vendors",
                newName: "IX_Vendors_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Presenter_MemberId",
                table: "Presenters",
                newName: "IX_Presenters_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Presentation_PresenterId",
                table: "Presentations",
                newName: "IX_Presentations_PresenterId");

            migrationBuilder.RenameIndex(
                name: "IX_Presentation_EventId",
                table: "Presentations",
                newName: "IX_Presentations_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVolunteer_VolunteerId",
                table: "EventVolunteers",
                newName: "IX_EventVolunteers_VolunteerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVolunteer_EventId",
                table: "EventVolunteers",
                newName: "IX_EventVolunteers_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVendor_VendorId",
                table: "EventVendors",
                newName: "IX_EventVendors_VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_EventVendor_EventId",
                table: "EventVendors",
                newName: "IX_EventVendors_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_VenueId",
                table: "Events",
                newName: "IX_Events_VenueId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendee_TicketId",
                table: "Attendees",
                newName: "IX_Attendees_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendee_MemberId",
                table: "Attendees",
                newName: "IX_Attendees_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Volunteers",
                table: "Volunteers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendors",
                table: "Vendors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Presenters",
                table: "Presenters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Presentations",
                table: "Presentations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventVolunteers",
                table: "EventVolunteers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventVendors",
                table: "EventVendors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClubMembers",
                table: "ClubMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendees",
                table: "Attendees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendees_Members_MemberId",
                table: "Attendees",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendees_Ticket_TicketId",
                table: "Attendees",
                column: "TicketId",
                principalTable: "Ticket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venue_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venue",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventVendors_Events_EventId",
                table: "EventVendors",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventVendors_Vendors_VendorId",
                table: "EventVendors",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventVolunteers_Events_EventId",
                table: "EventVolunteers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventVolunteers_Volunteers_VolunteerId",
                table: "EventVolunteers",
                column: "VolunteerId",
                principalTable: "Volunteers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Presentations_Events_EventId",
                table: "Presentations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Presentations_Presenters_PresenterId",
                table: "Presentations",
                column: "PresenterId",
                principalTable: "Presenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Presenters_Members_MemberId",
                table: "Presenters",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Members_MemberId",
                table: "Staff",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Events_EventId",
                table: "Ticket",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Members_MemberId",
                table: "User",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Members_MemberId",
                table: "Vendors",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Members_MemberId",
                table: "Volunteers",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitList_Attendees_AttendeeId",
                table: "WaitList",
                column: "AttendeeId",
                principalTable: "Attendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

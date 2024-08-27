using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class _82624_014_Prod_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Attendee",
                keyColumn: "Id",
                keyValue: 2468);

            migrationBuilder.DeleteData(
                table: "EventVendor",
                keyColumn: "Id",
                keyValue: 3333);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Presentation",
                keyColumn: "Id",
                keyValue: 2222);

            migrationBuilder.DeleteData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 12354);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Volunteer",
                keyColumn: "Id",
                keyValue: 1234);

            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: 7725);

            migrationBuilder.DeleteData(
                table: "Presenter",
                keyColumn: "Id",
                keyValue: 1234);

            migrationBuilder.DeleteData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 1234);

            migrationBuilder.DeleteData(
                table: "Vendor",
                keyColumn: "Id",
                keyValue: 1128);

            migrationBuilder.DeleteData(
                table: "Event",
                keyColumn: "Id",
                keyValue: 1111);

            migrationBuilder.DeleteData(
                table: "Venue",
                keyColumn: "Id",
                keyValue: 12345);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), "Admin Group", "Admin" });

            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "AdditionalInfo", "ClubId", "DateOfBirth", "Email", "FetName", "FirstName", "LastName", "PhoneNumber", "PresenterId", "PublicId", "VendorId" },
                values: new object[] { 7725, "This is a test user.", 12345, new DateOnly(1980, 1, 1), "John.Doe@example.com", "JohnDoe", "John", "Doe", "555-555-5555", null, 54321, null });

            migrationBuilder.InsertData(
                table: "Presenter",
                columns: new[] { "Id", "Bio", "Details", "Fee", "ImgPath", "LastAttended", "PublicName", "Requests" },
                values: new object[] { 1234, "This is a test presenter.", "Test Details", 100.00m, null, new DateOnly(2021, 1, 1), "Test Presenter", "Test Requests" });

            migrationBuilder.InsertData(
                table: "Vendor",
                columns: new[] { "Id", "Bio", "ImgPath", "LastAttended", "MerchType", "PriceAvg", "PriceMax", "PriceMin", "PublicName" },
                values: new object[] { 1128, "This is a test vendor.", "/wwwroot/images/Vendors/image01.jpg", new DateOnly(2021, 1, 1), "Test Merch", 5.00m, 10.00m, 1.00m, "Test Vendor" });

            migrationBuilder.InsertData(
                table: "Venue",
                columns: new[] { "Id", "Address", "Capacity", "City", "Cost", "Name", "State" },
                values: new object[] { 12345, "123 Test St.", 100, "Test City", null, "Test Venue", "TS" });

            migrationBuilder.InsertData(
                table: "Event",
                columns: new[] { "Id", "Description", "EndDate", "ImagePath", "Link", "Name", "StartDate", "Topic", "VenueId" },
                values: new object[] { 1111, "This is a test event.", new DateOnly(2021, 1, 2), null, null, "Test Event", new DateOnly(2021, 1, 1), null, 12345 });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "MemberId", "Password", "Token", "Username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), 7725, "password", null, "admin" });

            migrationBuilder.InsertData(
                table: "Volunteer",
                columns: new[] { "Id", "Details", "MemberId", "Positions" },
                values: new object[] { 1234, "Test Details", 7725, "[\"Test Position\"]" });

            migrationBuilder.InsertData(
                table: "EventVendor",
                columns: new[] { "Id", "ConfirmationNumber", "EventId", "IsPaid", "VendorId" },
                values: new object[] { 3333, 0, 1111, false, 1128 });

            migrationBuilder.InsertData(
                table: "Presentation",
                columns: new[] { "Id", "Description", "EventId", "ImgPath", "Name", "PresenterId", "Type" },
                values: new object[] { 2222, "This is a test presentation.", 1111, "/wwwroot/Presentations/image01.jpg", "Test Presentation", 1234, null });

            migrationBuilder.InsertData(
                table: "Ticket",
                columns: new[] { "Id", "DatePurchased", "EndDate", "EventId", "IsComped", "Price", "StartDate", "Type" },
                values: new object[,]
                {
                    { 1234, new DateOnly(2021, 1, 1), new DateOnly(2021, 1, 2), 1111, false, 10.0, new DateOnly(2021, 1, 1), "Test Ticket" },
                    { 12354, new DateOnly(2021, 1, 1), new DateOnly(2021, 1, 2), 1111, false, 10.0, new DateOnly(2021, 1, 1), "Test Ticket" }
                });

            migrationBuilder.InsertData(
                table: "Attendee",
                columns: new[] { "Id", "BackgroundChecked", "BadgeName", "ConfirmationNumber", "IsPaid", "MemberId", "RoomPreference", "RoomWaitListed", "TicketId", "TicketWaitListed", "isRegistered" },
                values: new object[] { 2468, true, "RandomNessy", 0, true, 7725, "Special", false, 1234, false, false });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class KiCDataInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClubMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Sex = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMembers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    FetName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClubId = table.Column<int>(type: "int", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublicId = table.Column<int>(type: "int", nullable: true),
                    AdditionalInfo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsVendor = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsVolunteer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPresenter = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsStaff = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Venue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venue", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Presenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    PublicName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Bio = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastAttended = table.Column<DateOnly>(type: "date", nullable: true),
                    Requests = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fee = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presenters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presenters_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staff_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    PublicName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Bio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastAttended = table.Column<DateOnly>(type: "date", nullable: true),
                    MerchType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PriceMin = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    PriceMax = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    PriceAvg = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    Positions = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Volunteers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Topic = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VenueId = table.Column<int>(type: "int", nullable: true),
                    ImagePath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Venue_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venue",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventVendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConfirmationNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventVendors_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventVendors_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventVolunteers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VolunteerId = table.Column<int>(type: "int", nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    ShiftNumber = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVolunteers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventVolunteers_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventVolunteers_Volunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Volunteers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Presentations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PresenterId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presentations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presentations_Presenters_PresenterId",
                        column: x => x.PresenterId,
                        principalTable: "Presenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatePurchased = table.Column<DateOnly>(type: "date", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsComped = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Attendees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    BadgeName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackgroundChecked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConfirmationNumber = table.Column<int>(type: "int", nullable: false),
                    RoomWaitListed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TicketWaitListed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RoomPreference = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPaid = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendees_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendees_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TicketComp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    CompReason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorizingUser = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketComp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketComp_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WaitList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttendeeId = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Comments = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitList_Attendees_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "Attendees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "AdditionalInfo", "ClubId", "DateOfBirth", "Email", "FetName", "FirstName", "IsPresenter", "IsStaff", "IsVendor", "IsVolunteer", "LastName", "PhoneNumber", "PublicId" },
                values: new object[] { 7725, "This is a test user.", 12345, new DateOnly(1980, 1, 1), "John.Doe@example.com", "JohnDoe", "John", false, false, false, true, "Doe", "555-555-5555", 54321 });

            migrationBuilder.InsertData(
                table: "Venue",
                columns: new[] { "Id", "Address", "Capacity", "City", "Name", "State" },
                values: new object[] { 12345, "123 Test St.", 100, "Test City", "Test Venue", "TS" });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Description", "EndDate", "ImagePath", "Name", "StartDate", "Topic", "VenueId" },
                values: new object[] { 1111, "This is a test event.", new DateOnly(2021, 1, 2), null, "Test Event", new DateOnly(2021, 1, 1), null, 12345 });

            migrationBuilder.InsertData(
                table: "Presenters",
                columns: new[] { "Id", "Bio", "Details", "Fee", "LastAttended", "MemberId", "PublicName", "Requests" },
                values: new object[] { 1234, "This is a test presenter.", "Test Details", 100.00m, new DateOnly(2021, 1, 1), 7725, "Test Presenter", "Test Requests" });

            migrationBuilder.InsertData(
                table: "Vendors",
                columns: new[] { "Id", "Bio", "LastAttended", "MemberId", "MerchType", "PriceAvg", "PriceMax", "PriceMin", "PublicName" },
                values: new object[] { 1128, "This is a test vendor.", new DateOnly(2021, 1, 1), 7725, "Test Merch", 5.00m, 10.00m, 1.00m, "Test Vendor" });

            migrationBuilder.InsertData(
                table: "Volunteers",
                columns: new[] { "Id", "Details", "MemberId", "Positions" },
                values: new object[] { 1234, "Test Details", 7725, "[\"Test Position\"]" });

            migrationBuilder.InsertData(
                table: "EventVendors",
                columns: new[] { "Id", "ConfirmationNumber", "EventId", "IsPaid", "VendorId" },
                values: new object[] { 3333, 0, 1111, false, 1128 });

            migrationBuilder.InsertData(
                table: "EventVolunteers",
                columns: new[] { "Id", "EventId", "Position", "ShiftNumber", "VolunteerId" },
                values: new object[] { 3579, 1111, null, null, 1234 });

            migrationBuilder.InsertData(
                table: "Presentations",
                columns: new[] { "Id", "Description", "EventId", "Name", "PresenterId", "Type" },
                values: new object[] { 2222, "This is a test presentation.", 1111, "Test Presentation", 1234, null });

            migrationBuilder.InsertData(
                table: "Ticket",
                columns: new[] { "Id", "DatePurchased", "EndDate", "EventId", "IsComped", "Name", "Price", "StartDate", "Type" },
                values: new object[,]
                {
                    { 1234, new DateOnly(2021, 1, 1), new DateOnly(2021, 1, 2), 1111, false, "Test Ticket", 10.00m, new DateOnly(2021, 1, 1), "Test Ticket" },
                    { 12354, new DateOnly(2021, 1, 1), new DateOnly(2021, 1, 2), 1111, false, "Test Ticket", 10.00m, new DateOnly(2021, 1, 1), "Test Ticket" }
                });

            migrationBuilder.InsertData(
                table: "Attendees",
                columns: new[] { "Id", "BackgroundChecked", "BadgeName", "ConfirmationNumber", "IsPaid", "MemberId", "RoomPreference", "RoomWaitListed", "TicketId", "TicketWaitListed" },
                values: new object[] { 2468, true, "RandomNessy", 0, true, 7725, "Special", false, 1234, false });

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_MemberId",
                table: "Attendees",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_TicketId",
                table: "Attendees",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_VenueId",
                table: "Events",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVendors_EventId",
                table: "EventVendors",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVendors_VendorId",
                table: "EventVendors",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVolunteers_EventId",
                table: "EventVolunteers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVolunteers_VolunteerId",
                table: "EventVolunteers",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentations_EventId",
                table: "Presentations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentations_PresenterId",
                table: "Presentations",
                column: "PresenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Presenters_MemberId",
                table: "Presenters",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_MemberId",
                table: "Staff",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_EventId",
                table: "Ticket",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComp_TicketId",
                table: "TicketComp",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_User_MemberId",
                table: "User",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_MemberId",
                table: "Vendors",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_MemberId",
                table: "Volunteers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitList_AttendeeId",
                table: "WaitList",
                column: "AttendeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubMembers");

            migrationBuilder.DropTable(
                name: "EventVendors");

            migrationBuilder.DropTable(
                name: "EventVolunteers");

            migrationBuilder.DropTable(
                name: "Presentations");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "TicketComp");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "WaitList");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "Volunteers");

            migrationBuilder.DropTable(
                name: "Presenters");

            migrationBuilder.DropTable(
                name: "Attendees");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Venue");
        }
    }
}

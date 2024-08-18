using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class KiCModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Attendee_TicketId",
            //    table: "Attendee");

            migrationBuilder.AddColumn<decimal>(
                name: "CompAmount",
                table: "TicketComp",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Member",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isRegistered",
                table: "Attendee",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PendingVolunteers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VolunteerID = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    PreferredShift = table.Column<int>(type: "int", nullable: false),
                    PreferredPositions = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingVolunteers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Attendee",
                keyColumn: "Id",
                keyValue: 2468,
                column: "isRegistered",
                value: false);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Attendee_TicketId",
            //    table: "Attendee",
            //    column: "TicketId",
            //    unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingVolunteers");

            migrationBuilder.DropIndex(
                name: "IX_Attendee_TicketId",
                table: "Attendee");

            migrationBuilder.DropColumn(
                name: "CompAmount",
                table: "TicketComp");

            migrationBuilder.DropColumn(
                name: "isRegistered",
                table: "Attendee");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Member",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_Attendee_TicketId",
                table: "Attendee",
                column: "TicketId");
        }
    }
}

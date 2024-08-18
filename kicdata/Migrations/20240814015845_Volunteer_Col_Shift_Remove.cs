using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class Volunteer_Col_Shift_Remove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventVolunteer",
                keyColumn: "Id",
                keyValue: 3579);

            migrationBuilder.DropColumn(
                name: "Shifts",
                table: "Volunteer");

            migrationBuilder.DropColumn(
                name: "PreferredShift",
                table: "PendingVolunteers");

            migrationBuilder.DropColumn(
                name: "ShiftNumber",
                table: "EventVolunteer");

            migrationBuilder.AddColumn<string>(
                name: "Shift",
                table: "EventVolunteer",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shift",
                table: "EventVolunteer");

            migrationBuilder.AddColumn<string>(
                name: "Shifts",
                table: "Volunteer",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PreferredShift",
                table: "PendingVolunteers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShiftNumber",
                table: "EventVolunteer",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "EventVolunteer",
                columns: new[] { "Id", "EventId", "Position", "ShiftNumber", "VolunteerId" },
                values: new object[] { 3579, 1111, null, null, 1234 });

            migrationBuilder.UpdateData(
                table: "Volunteer",
                keyColumn: "Id",
                keyValue: 1234,
                column: "Shifts",
                value: null);
        }
    }
}

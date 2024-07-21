using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class KiCUpdateTicketProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Ticket");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "WaitList",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Shifts",
                table: "Volunteers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "Cost",
                table: "Venue",
                type: "float",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Venue",
                keyColumn: "Id",
                keyValue: 12345,
                column: "Cost",
                value: null);

            migrationBuilder.UpdateData(
                table: "Volunteers",
                keyColumn: "Id",
                keyValue: 1234,
                column: "Shifts",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shifts",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Venue");

            migrationBuilder.UpdateData(
                table: "WaitList",
                keyColumn: "Comments",
                keyValue: null,
                column: "Comments",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "WaitList",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Ticket",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 1234,
                column: "Name",
                value: "Test Ticket");

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 12354,
                column: "Name",
                value: "Test Ticket");
        }
    }
}

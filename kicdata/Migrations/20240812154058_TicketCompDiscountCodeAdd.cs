using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class TicketCompDiscountCodeAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AuthorizingUser",
                table: "TicketComp",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "TicketComp",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PendingVolunteers_EventId",
                table: "PendingVolunteers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingVolunteers_VolunteerID",
                table: "PendingVolunteers",
                column: "VolunteerID");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingVolunteers_Event_EventId",
                table: "PendingVolunteers",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PendingVolunteers_Volunteer_VolunteerID",
                table: "PendingVolunteers",
                column: "VolunteerID",
                principalTable: "Volunteer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingVolunteers_Event_EventId",
                table: "PendingVolunteers");

            migrationBuilder.DropForeignKey(
                name: "FK_PendingVolunteers_Volunteer_VolunteerID",
                table: "PendingVolunteers");

            migrationBuilder.DropIndex(
                name: "IX_PendingVolunteers_EventId",
                table: "PendingVolunteers");

            migrationBuilder.DropIndex(
                name: "IX_PendingVolunteers_VolunteerID",
                table: "PendingVolunteers");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "TicketComp");

            migrationBuilder.UpdateData(
                table: "TicketComp",
                keyColumn: "AuthorizingUser",
                keyValue: null,
                column: "AuthorizingUser",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorizingUser",
                table: "TicketComp",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}

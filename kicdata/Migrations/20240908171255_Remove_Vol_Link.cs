using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Vol_Link : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingVolunteers_Volunteer_VolunteerID",
                table: "PendingVolunteers");

            migrationBuilder.DropIndex(
                name: "IX_PendingVolunteers_VolunteerID",
                table: "PendingVolunteers");

            migrationBuilder.DropColumn(
                name: "VolunteerID",
                table: "PendingVolunteers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VolunteerID",
                table: "PendingVolunteers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PendingVolunteers_VolunteerID",
                table: "PendingVolunteers",
                column: "VolunteerID");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingVolunteers_Volunteer_VolunteerID",
                table: "PendingVolunteers",
                column: "VolunteerID",
                principalTable: "Volunteer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

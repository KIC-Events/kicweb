using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class _015Beta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "PendingVolunteers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "PendingVolunteers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "PendingVolunteers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "PendingVolunteers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "PendingVolunteers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "PendingVolunteers");
        }
    }
}

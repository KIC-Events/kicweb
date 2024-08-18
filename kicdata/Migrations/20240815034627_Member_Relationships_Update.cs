using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiCData.Migrations
{
    /// <inheritdoc />
    public partial class Member_Relationships_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presenter_Member_MemberId",
                table: "Presenter");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendor_Member_MemberId",
                table: "Vendor");

            //migrationBuilder.DropIndex(
            //    name: "IX_Volunteer_MemberId",
            //    table: "Volunteer");

            migrationBuilder.DropIndex(
                name: "IX_Vendor_MemberId",
                table: "Vendor");

            migrationBuilder.DropIndex(
                name: "IX_User_MemberId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Staff_MemberId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Presenter_MemberId",
                table: "Presenter");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Presenter");

            migrationBuilder.DropColumn(
                name: "IsPresenter",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "IsStaff",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "IsVendor",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "IsVolunteer",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "Shift",
                table: "EventVolunteer");

            migrationBuilder.AddColumn<int>(
                name: "PresenterId",
                table: "Member",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Member",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftEnd",
                table: "EventVolunteer",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftStart",
                table: "EventVolunteer",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: 7725,
                columns: new[] { "PresenterId", "VendorId" },
                values: new object[] { null, null });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Volunteer_MemberId",
            //    table: "Volunteer",
            //    column: "MemberId",
            //    unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_MemberId",
                table: "User",
                column: "MemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_MemberId",
                table: "Staff",
                column: "MemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_PresenterId",
                table: "Member",
                column: "PresenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_VendorId",
                table: "Member",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Presenter_PresenterId",
                table: "Member",
                column: "PresenterId",
                principalTable: "Presenter",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Vendor_VendorId",
                table: "Member",
                column: "VendorId",
                principalTable: "Vendor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_Presenter_PresenterId",
                table: "Member");

            migrationBuilder.DropForeignKey(
                name: "FK_Member_Vendor_VendorId",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Volunteer_MemberId",
                table: "Volunteer");

            migrationBuilder.DropIndex(
                name: "IX_User_MemberId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Staff_MemberId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Member_PresenterId",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_VendorId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "PresenterId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "ShiftEnd",
                table: "EventVolunteer");

            migrationBuilder.DropColumn(
                name: "ShiftStart",
                table: "EventVolunteer");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Vendor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Presenter",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPresenter",
                table: "Member",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStaff",
                table: "Member",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVendor",
                table: "Member",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVolunteer",
                table: "Member",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Shift",
                table: "EventVolunteer",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: 7725,
                columns: new[] { "IsPresenter", "IsStaff", "IsVendor", "IsVolunteer" },
                values: new object[] { false, false, false, true });

            migrationBuilder.UpdateData(
                table: "Presenter",
                keyColumn: "Id",
                keyValue: 1234,
                column: "MemberId",
                value: 7725);

            migrationBuilder.UpdateData(
                table: "Vendor",
                keyColumn: "Id",
                keyValue: 1128,
                column: "MemberId",
                value: 7725);

            migrationBuilder.CreateIndex(
                name: "IX_Volunteer_MemberId",
                table: "Volunteer",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_MemberId",
                table: "Vendor",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_User_MemberId",
                table: "User",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_MemberId",
                table: "Staff",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Presenter_MemberId",
                table: "Presenter",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presenter_Member_MemberId",
                table: "Presenter",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendor_Member_MemberId",
                table: "Vendor",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");
        }
    }
}

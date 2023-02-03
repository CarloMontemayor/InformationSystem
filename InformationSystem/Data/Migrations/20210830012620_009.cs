using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _009 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BarangayOfficial");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "BarangayOfficial",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "BarangayOfficial",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBarangayOfficial",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Jail_BarangayId",
                table: "Jail",
                column: "BarangayId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinic_BarangayId",
                table: "Clinic",
                column: "BarangayId");

            migrationBuilder.CreateIndex(
                name: "IX_BarangayOfficial_UserId1",
                table: "BarangayOfficial",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BarangayOfficial_AspNetUsers_UserId1",
                table: "BarangayOfficial",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clinic_Barangay_BarangayId",
                table: "Clinic",
                column: "BarangayId",
                principalTable: "Barangay",
                principalColumn: "BarangayId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jail_Barangay_BarangayId",
                table: "Jail",
                column: "BarangayId",
                principalTable: "Barangay",
                principalColumn: "BarangayId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarangayOfficial_AspNetUsers_UserId1",
                table: "BarangayOfficial");

            migrationBuilder.DropForeignKey(
                name: "FK_Clinic_Barangay_BarangayId",
                table: "Clinic");

            migrationBuilder.DropForeignKey(
                name: "FK_Jail_Barangay_BarangayId",
                table: "Jail");

            migrationBuilder.DropIndex(
                name: "IX_Jail_BarangayId",
                table: "Jail");

            migrationBuilder.DropIndex(
                name: "IX_Clinic_BarangayId",
                table: "Clinic");

            migrationBuilder.DropIndex(
                name: "IX_BarangayOfficial_UserId1",
                table: "BarangayOfficial");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BarangayOfficial");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "BarangayOfficial");

            migrationBuilder.DropColumn(
                name: "IsBarangayOfficial",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BarangayOfficial",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

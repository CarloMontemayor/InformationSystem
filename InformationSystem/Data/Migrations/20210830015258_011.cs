using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarangayOfficial_AspNetUsers_UserId1",
                table: "BarangayOfficial");

            migrationBuilder.DropIndex(
                name: "IX_BarangayOfficial_UserId1",
                table: "BarangayOfficial");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "BarangayOfficial");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BarangayOfficial",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_BarangayOfficial_UserId",
                table: "BarangayOfficial",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarangayOfficial_AspNetUsers_UserId",
                table: "BarangayOfficial",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarangayOfficial_AspNetUsers_UserId",
                table: "BarangayOfficial");

            migrationBuilder.DropIndex(
                name: "IX_BarangayOfficial_UserId",
                table: "BarangayOfficial");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "BarangayOfficial",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "BarangayOfficial",
                type: "nvarchar(450)",
                nullable: true);

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
        }
    }
}

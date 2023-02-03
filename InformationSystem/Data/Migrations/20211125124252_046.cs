using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _046 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Respondent",
                table: "AccidentProne",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "AccidentProne",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "When",
                table: "AccidentProne",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Where",
                table: "AccidentProne",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Respondent",
                table: "AccidentProne");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "AccidentProne");

            migrationBuilder.DropColumn(
                name: "When",
                table: "AccidentProne");

            migrationBuilder.DropColumn(
                name: "Where",
                table: "AccidentProne");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _044 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AccidentProne",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AccidentProne");
        }
    }
}

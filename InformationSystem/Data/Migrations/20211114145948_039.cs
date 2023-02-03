using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _039 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Disease",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Crime",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disease_UserId",
                table: "Disease",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Crime_UserId",
                table: "Crime",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Crime_AspNetUsers_UserId",
                table: "Crime",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disease_AspNetUsers_UserId",
                table: "Disease",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Crime_AspNetUsers_UserId",
                table: "Crime");

            migrationBuilder.DropForeignKey(
                name: "FK_Disease_AspNetUsers_UserId",
                table: "Disease");

            migrationBuilder.DropIndex(
                name: "IX_Disease_UserId",
                table: "Disease");

            migrationBuilder.DropIndex(
                name: "IX_Crime_UserId",
                table: "Crime");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Disease");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Crime");
        }
    }
}

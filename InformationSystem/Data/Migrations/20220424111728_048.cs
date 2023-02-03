using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _048 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailsSuggestion",
                table: "FeedBacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingService",
                table: "FeedBacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsSuggestion",
                table: "FeedBacks");

            migrationBuilder.DropColumn(
                name: "RatingService",
                table: "FeedBacks");
        }
    }
}

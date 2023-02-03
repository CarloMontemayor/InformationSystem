using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrimeCases",
                columns: table => new
                {
                    CrimeCasesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrimeId = table.Column<int>(type: "int", nullable: false),
                    JailId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrimeCases", x => x.CrimeCasesId);
                    table.ForeignKey(
                        name: "FK_CrimeCases_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CrimeCases_Crime_CrimeId",
                        column: x => x.CrimeId,
                        principalTable: "Crime",
                        principalColumn: "CrimeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrimeCases_Jail_JailId",
                        column: x => x.JailId,
                        principalTable: "Jail",
                        principalColumn: "JailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrimeCases_CrimeId",
                table: "CrimeCases",
                column: "CrimeId");

            migrationBuilder.CreateIndex(
                name: "IX_CrimeCases_JailId",
                table: "CrimeCases",
                column: "JailId");

            migrationBuilder.CreateIndex(
                name: "IX_CrimeCases_UserId",
                table: "CrimeCases",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrimeCases");
        }
    }
}

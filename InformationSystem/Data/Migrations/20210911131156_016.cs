using Microsoft.EntityFrameworkCore.Migrations;

namespace InformationSystem.Data.Migrations
{
    public partial class _016 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrimeCases_Jail_JailId",
                table: "CrimeCases");

            migrationBuilder.DropForeignKey(
                name: "FK_HealthCases_Clinic_ClinicId",
                table: "HealthCases");

            migrationBuilder.DropTable(
                name: "Clinic");

            migrationBuilder.DropTable(
                name: "Jail");

            migrationBuilder.DropIndex(
                name: "IX_HealthCases_ClinicId",
                table: "HealthCases");

            migrationBuilder.DropIndex(
                name: "IX_CrimeCases_JailId",
                table: "CrimeCases");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "HealthCases");

            migrationBuilder.DropColumn(
                name: "JailId",
                table: "CrimeCases");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "HealthCases",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "HealthCases",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "CrimeCases",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "CrimeCases",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "HealthCases");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "HealthCases");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "CrimeCases");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "CrimeCases");

            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "HealthCases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JailId",
                table: "CrimeCases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Clinic",
                columns: table => new
                {
                    ClinicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarangayId = table.Column<int>(type: "int", nullable: false),
                    ClinicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinic", x => x.ClinicId);
                    table.ForeignKey(
                        name: "FK_Clinic_Barangay_BarangayId",
                        column: x => x.BarangayId,
                        principalTable: "Barangay",
                        principalColumn: "BarangayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jail",
                columns: table => new
                {
                    JailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarangayId = table.Column<int>(type: "int", nullable: false),
                    JailName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jail", x => x.JailId);
                    table.ForeignKey(
                        name: "FK_Jail_Barangay_BarangayId",
                        column: x => x.BarangayId,
                        principalTable: "Barangay",
                        principalColumn: "BarangayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthCases_ClinicId",
                table: "HealthCases",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_CrimeCases_JailId",
                table: "CrimeCases",
                column: "JailId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinic_BarangayId",
                table: "Clinic",
                column: "BarangayId");

            migrationBuilder.CreateIndex(
                name: "IX_Jail_BarangayId",
                table: "Jail",
                column: "BarangayId");

            migrationBuilder.AddForeignKey(
                name: "FK_CrimeCases_Jail_JailId",
                table: "CrimeCases",
                column: "JailId",
                principalTable: "Jail",
                principalColumn: "JailId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HealthCases_Clinic_ClinicId",
                table: "HealthCases",
                column: "ClinicId",
                principalTable: "Clinic",
                principalColumn: "ClinicId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWaterMeterAndClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "WaterMeters");

            migrationBuilder.AddColumn<int>(
                name: "WaterMeterServicesId",
                table: "WaterMeters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterMeterServicesId",
                table: "WaterMeters");

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "WaterMeters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

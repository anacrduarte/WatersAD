using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientDataAndWaterMeterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "WaterMeters",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemainPostalCode",
                table: "WaterMeters",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "Clients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Clients",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemainPostalCode",
                table: "Clients",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "WaterMeters");

            migrationBuilder.DropColumn(
                name: "RemainPostalCode",
                table: "WaterMeters");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RemainPostalCode",
                table: "Clients");
        }
    }
}

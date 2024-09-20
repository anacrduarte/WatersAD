using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLocalityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Localities");

            migrationBuilder.DropColumn(
                name: "RemainPostalCode",
                table: "Localities");

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Localities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Localities");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Localities",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemainPostalCode",
                table: "Localities",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);
        }
    }
}

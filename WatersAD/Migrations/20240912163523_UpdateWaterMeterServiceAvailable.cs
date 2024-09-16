using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWaterMeterServiceAvailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "WaterMeterServices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "WaterMeterServices");
        }
    }
}

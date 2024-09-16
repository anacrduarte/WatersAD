using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWaterMeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterMeters_WaterMeterServices_WaterMeterServiceId",
                table: "WaterMeters");

            migrationBuilder.DropColumn(
                name: "WaterMeterServicesId",
                table: "WaterMeters");

            migrationBuilder.AlterColumn<int>(
                name: "WaterMeterServiceId",
                table: "WaterMeters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterMeters_WaterMeterServices_WaterMeterServiceId",
                table: "WaterMeters",
                column: "WaterMeterServiceId",
                principalTable: "WaterMeterServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterMeters_WaterMeterServices_WaterMeterServiceId",
                table: "WaterMeters");

            migrationBuilder.AlterColumn<int>(
                name: "WaterMeterServiceId",
                table: "WaterMeters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "WaterMeterServicesId",
                table: "WaterMeters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterMeters_WaterMeterServices_WaterMeterServiceId",
                table: "WaterMeters",
                column: "WaterMeterServiceId",
                principalTable: "WaterMeterServices",
                principalColumn: "Id");
        }
    }
}
